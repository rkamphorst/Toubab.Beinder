using System;
using System.Collections.Generic;
using Toubab.Beinder.Bindable;
using Toubab.Beinder.Extend;
using Toubab.Beinder.Mixin;
using Toubab.Beinder.Annex;

namespace Toubab.Beinder.Scanner
{
    public class MixinScanner : AdapterScanner<IMixin>
    {
        readonly IScanner _scanner;

        public MixinScanner(IScanner scanner)
        {
            _scanner = scanner;
        }

        public override IEnumerable<IBindable> Scan(Type type)
        {
            foreach (var ext in AdapterFactory.GetAdaptersFor(type))
            {
                foreach (IBindable prop in _scanner.Scan(ext))
                {
                    prop.SetObject(ext);

                    var sprop = prop as IProperty;
                    if (sprop != null)
                    {
                        yield return new DelegatedProperty(sprop);
                        continue;
                    }

                    var pprop = prop as IEvent;
                    if (pprop != null)
                    {
                        yield return new DelegatedEvent(pprop);
                        continue;
                    }

                    var cprop = prop as IEventHandler;
                    if (cprop != null)
                    {
                        yield return new DelegatedEventHandler(cprop);
                        continue;
                    }

                }
            }
        }

    }
}

