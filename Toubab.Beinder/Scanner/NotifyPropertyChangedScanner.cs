using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.ComponentModel;
using Toubab.Beinder.PathParser;
using Toubab.Beinder.Valve;

namespace Toubab.Beinder.Scanner
{

    public class NotifyPropertyChangedScanner : TypePropertyScanner
    {
     
        IPathParser _pathParser = new CamelCasePathParser();

        public IPathParser PathParser
        { 
            get { return _pathParser; }
            set { _pathParser = value; }
        }

        public override IEnumerable<IBindableState> Scan(Type type)
        {
            var isNotifyPropertyChanged =
                typeof(INotifyPropertyChanged).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());

            if (!isNotifyPropertyChanged)
                return Enumerable.Empty<IBindableState>();
            

            return
                type.GetRuntimeProperties()
                    .Where(info => 
                        !info.IsSpecialName &&
                        info.GetIndexParameters().Length == 0 &&
                        ((info.GetMethod != null && info.GetMethod.IsPublic) ||
                         (info.SetMethod != null && info.SetMethod.IsPublic))
                    )
                    .Select(prop => new NotifyPropertyChangedTypeProperty(_pathParser, prop)
                );
        }

        class NotifyPropertyChangedTypeProperty : TypeProperty
        {
            public NotifyPropertyChangedTypeProperty(IPathParser pathParser, PropertyInfo property)
                : base(pathParser, property)
            {
            }

            void HandlePropertyChanged(object source, PropertyChangedEventArgs args)
            {
                if (Equals(args.PropertyName, _propertyInfo.Name))
                {
                    OnBroadcast(this, EventArgs.Empty);
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

            public override IBindable CloneWithoutObject()
            {
                return new NotifyPropertyChangedTypeProperty(_pathParser, _propertyInfo);
            }
                
        }

    }
    
}
