using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Toubab.Beinder.Bindable;
using Toubab.Beinder.PathParser;
using Toubab.Beinder.Extend;

namespace Toubab.Beinder.Scanner
{

    public class ReflectionScanner : TypeScanner
    {
        IPathParser _pathParser = new CamelCasePathParser();

        public IPathParser PathParser
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
                    .Select(prop => (IBindable) new ReflectedProperty(_pathParser, prop, type.GetRuntimeEvent(prop.Name + "Changed")))
            .Concat(
                type.GetRuntimeMethods()
                    .Where(info => !info.IsSpecialName && info.IsPublic && info.ReturnType == typeof(void))
                    .Select(method => (IBindable) new ReflectedEventHandler(_pathParser, method))
            )
            .Concat(
                type.GetRuntimeEvents()
                    .Where(info => !info.IsSpecialName && info.AddMethod.IsPublic)
                    .Select(evt => (IBindable) new ReflectedEvent(_pathParser, evt))
            );
        }

    }


}
