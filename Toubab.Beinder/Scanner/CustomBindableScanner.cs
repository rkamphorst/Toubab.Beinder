using System;
using System.Collections.Generic;
using Toubab.Beinder.Bindable;
using Toubab.Beinder.Extend;

namespace Toubab.Beinder.Scanner
{
    public class CustomBindableScanner : AdapterScanner<ICustomBindable>
    {
        public override IEnumerable<IBindable> Scan(Type type)
        {
            foreach (var prop in AdapterFactory.GetAdaptersFor(type))
            {
                yield return prop;
            }
        }
    }
}

