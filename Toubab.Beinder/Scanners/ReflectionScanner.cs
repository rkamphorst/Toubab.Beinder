namespace Toubab.Beinder.Scanners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Bindables;
    using Paths;

    public class ReflectionScanner : TypeScanner
    {
        IFragmentParser _pathParser = new CamelCaseSyllableParser();

        public IFragmentParser PathParser
        { 
            get { return _pathParser; }
            set { _pathParser = value; }
        }

        public override IEnumerable<IBindable> Scan(Type type)
        {
            return
                type.GetRuntimeProperties()
                    .Where(info => 
                        !info.IsSpecialName &&
                        info.GetIndexParameters().Length == 0 &&
                        ((info.GetMethod != null && info.GetMethod.IsPublic) ||
                         (info.SetMethod != null && info.SetMethod.IsPublic))
                    )
                    .Select(prop => (IBindable) new ReflectedProperty(
                            _pathParser.Parse(prop.Name), 
                            prop, 
                            type.GetRuntimeEvent(prop.Name + "Changed")
                        )
                    )
            .Concat(
                type.GetRuntimeMethods()
                    .Where(info => !info.IsSpecialName && info.IsPublic && info.ReturnType == typeof(void))
                    .Select(method => (IBindable) new ReflectedEventHandler(_pathParser.Parse(method.Name), method))
            )
            .Concat(
                type.GetRuntimeEvents()
                    .Where(info => !info.IsSpecialName && info.AddMethod.IsPublic)
                    .Select(evt => (IBindable) new ReflectedEvent(_pathParser.Parse(evt.Name), evt, null))
            );
        }

    }


}
