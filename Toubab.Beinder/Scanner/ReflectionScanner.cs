using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Toubab.Beinder.PathParser;
using Toubab.Beinder.Valve;

namespace Toubab.Beinder.Scanner
{

    public class ReflectionScanner : TypePropertyScanner
    {
        IPathParser _pathParser = new CamelCasePathParser();

        public IPathParser PathParser
        { 
            get { return _pathParser; }
            set { _pathParser = value; }
        }

        public override IEnumerable<IBindableState> Scan(Type type)
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

            public ReflectionTypeProperty(IPathParser pathParser, PropertyInfo property, EventInfo evt)
                : base(pathParser, property)
            {
                if (evt != null && evt.EventHandlerType == typeof(EventHandler))
                    _event = evt;
                _onValueChanged = new EventHandler(OnBroadcast);
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

            public override IBindable CloneWithoutObject()
            {
                return new ReflectionTypeProperty(_pathParser, _propertyInfo, _event);
            }

        }
    }


}
