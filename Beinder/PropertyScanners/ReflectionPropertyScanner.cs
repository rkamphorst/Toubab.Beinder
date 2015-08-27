using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Beinder.PropertyPathParsers;

namespace Beinder.PropertyScanners
{

    public class ReflectionPropertyScanner : ITypePropertyScanner
    {
        IPropertyPathParser _pathParser = new CamelCasePropertyPathParser();

        public IPropertyPathParser PathParser
        { 
            get { return _pathParser; }
            set { _pathParser = value; }
        }

        public IEnumerable<IProperty> Scan(Type type)
        {
            return
                type.GetRuntimeProperties()
                    .Where(info => 
                        !info.IsSpecialName &&
                info.GetIndexParameters().Length == 0 &&
                ((info.GetMethod != null && info.GetMethod.IsPublic) ||
                (info.SetMethod != null && info.GetMethod.IsPublic))
            )
                    .Select(prop => new ReflectionTypeProperty(type, _pathParser, prop, type.GetRuntimeEvent(prop.Name + "Changed"))
            );
        }

        class ReflectionTypeProperty : TypeProperty
        {
            readonly EventInfo _event;
            readonly EventHandler _onValueChanged;

            public ReflectionTypeProperty(Type type, IPropertyPathParser pathParser, PropertyInfo property, EventInfo evt)
                : base(type, pathParser, property)
            {
                if (evt != null && evt.EventHandlerType == typeof(EventHandler))
                    _event = evt;
                _onValueChanged = new EventHandler(OnValueChanged);
            }

            protected override void DetachObjectPropertyChangeEvent(object obj)
            {
                if (_event != null && obj != null)
                    _event.RemoveEventHandler(obj, _onValueChanged);
            }

            protected override void AttachObjectPropertyChangeEvent(object obj)
            {
                if (_event != null && obj != null)
                    _event.AddEventHandler(obj, _onValueChanged);
            }

            public override IProperty Clone()
            {
                var result = new ReflectionTypeProperty(MetaInfo.ObjectType, _pathParser, _propertyInfo, _event);
                result.TrySetObject(Object);
                return result;
            }

        }
    }


}
