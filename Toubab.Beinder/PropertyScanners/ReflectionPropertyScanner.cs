using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Toubab.Beinder.PropertyPathParsers;

namespace Toubab.Beinder.PropertyScanners
{

    public class ReflectionPropertyScanner : TypePropertyScanner
    {
        IPropertyPathParser _pathParser = new CamelCasePropertyPathParser();

        public IPropertyPathParser PathParser
        { 
            get { return _pathParser; }
            set { _pathParser = value; }
        }

        public override IEnumerable<IProperty> Scan(Type type)
        {
            return
                type.GetRuntimeProperties()
                    .Where(info => 
                        !info.IsSpecialName &&
                        info.GetIndexParameters().Length == 0 &&
                        ((info.GetMethod != null && info.GetMethod.IsPublic) ||
                         (info.SetMethod != null && info.SetMethod.IsPublic))
            )
                    .Select(prop => new ReflectionTypeProperty(_pathParser, prop, type.GetRuntimeEvent(prop.Name + "Changed"))
            );
        }

        class ReflectionTypeProperty : TypeProperty
        {
            readonly EventInfo _event;
            readonly EventHandler _onValueChanged;

            public ReflectionTypeProperty(IPropertyPathParser pathParser, PropertyInfo property, EventInfo evt)
                : base(pathParser, property)
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

            public override IProperty CloneWithoutObject()
            {
                return new ReflectionTypeProperty(_pathParser, _propertyInfo, _event);
            }

        }
    }


}
