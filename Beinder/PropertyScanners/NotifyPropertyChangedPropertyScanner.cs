using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.ComponentModel;
using Beinder.PropertyPathParsers;

namespace Beinder.PropertyScanners
{

    public class NotifyPropertyChangedPropertyScanner : ITypePropertyScanner
    {
     
        IPropertyPathParser _pathParser = new CamelCasePropertyPathParser();

        public IPropertyPathParser PathParser
        { 
            get { return _pathParser; }
            set { _pathParser = value; }
        }

        public IEnumerable<IProperty> Scan(Type type)
        {
            var isNotifyPropertyChanged =
                typeof(INotifyPropertyChanged).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());

            if (!isNotifyPropertyChanged)
                return Enumerable.Empty<IProperty>();
            

            return
                type.GetRuntimeProperties()
                    .Where(info => 
                        !info.IsSpecialName &&
                info.GetIndexParameters().Length == 0 &&
                ((info.GetMethod != null && info.GetMethod.IsPublic) ||
                (info.SetMethod != null && info.GetMethod.IsPublic))
            )
                    .Select(prop => new NotifyPropertyChangedTypeProperty(type, _pathParser, prop)
            );
        }

        class NotifyPropertyChangedTypeProperty : TypeProperty
        {
            public NotifyPropertyChangedTypeProperty(Type type, IPropertyPathParser pathParser, PropertyInfo property)
                : base(type, pathParser, property)
            {
            }

            void HandlePropertyChanged(object source, PropertyChangedEventArgs args)
            {
                if (Equals(args.PropertyName, _propertyInfo.Name))
                {
                    base.OnValueChanged(this, EventArgs.Empty);
                }
            }

            protected override void DetachObjectPropertyChangeEvent(object obj)
            {
                var inpc = Object as INotifyPropertyChanged;
                if (inpc != null)
                    inpc.PropertyChanged -= HandlePropertyChanged;
            }

            protected override void AttachObjectPropertyChangeEvent(object obj)
            {
                var inpc = Object as INotifyPropertyChanged;
                if (inpc != null)
                    inpc.PropertyChanged += HandlePropertyChanged;
            }

            public override IProperty Clone()
            {
                var result = new NotifyPropertyChangedTypeProperty(MetaInfo.ObjectType, _pathParser, _propertyInfo);
                result.TrySetObject(Object);
                return result;
            }
                
        }

    }
    
}
