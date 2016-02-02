namespace Toubab.Beinder.Scanners
{
    using System;
    using System.Collections.Generic;
    using Bindables;
    using Extend;

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

