namespace Toubab.Beinder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Bindables;
    using Scanners;
    using Mixins;
    using Valves;

    public class Binder2
    {
        public static CombinedScanner CreateDefaultCombinedScanner()
        {
            var result = new CombinedScanner();
            result.Add(new ReflectionScanner());
            result.Add(new NotifyPropertyChangedScanner());
            result.Add(new DictionaryScanner());

            var mixinScanner = new CustomMixinScanner(result);
            mixinScanner.AdapterRegistry.Register<CommandMixin>();
            result.Add(mixinScanner);
            
            result.Add(new CustomBindableScanner());
            return result;
        }

        readonly CombinedScanner _propertyScanner;

        public Binder2()
            : this(CreateDefaultCombinedScanner())
        {
        }

        public Binder2(CombinedScanner combinedScanner)
        {
            _propertyScanner = combinedScanner;

        }

        public Binder2(params IScanner[] scanners)
        {
            var combinedScanner = new CombinedScanner();
            foreach (var s in scanners)
            {
                var cs = s as CombinedScanner;
                if (cs != null)
                {
                    foreach (var s2 in cs)
                    {
                        combinedScanner.Add(s2);
                    }
                }
                else
                {
                    combinedScanner.Add(s);
                }
            }
            _propertyScanner = combinedScanner;
        }
            
        public CombinedScanner Scanner { get { return _propertyScanner; } }



    }
}
