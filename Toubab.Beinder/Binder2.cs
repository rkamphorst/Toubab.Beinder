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
            foreach (var ps in scanners)
            {
                var cps = ps as CombinedScanner;
                if (cps != null)
                {
                    foreach (var ps2 in cps)
                    {
                        combinedScanner.Add(ps2);
                    }
                }
                else
                {
                    combinedScanner.Add(ps);
                }
            }
            _propertyScanner = combinedScanner;
        }
            
        public CombinedScanner Scanner { get { return _propertyScanner; } }

        public async Task<Valve2[]> BindAsync(object[] objects, int tagToActivate = 0)
        {
            var fixtures = Fixture.CreateFixtures(_propertyScanner, objects);
            return await BindAsync(fixtures, tagToActivate);
        }

        async Task<Valve2[]> BindAsync(IEnumerable<Fixture> fixtures, int tagToActivate)
        {
            var resultList = new List<Valve2>();

            foreach (var fixture in fixtures) 
            {
                if (fixture.Conduits.Any(c => c.Bindable is IProperty))
                {
                    var newValve = new StateValve2(fixture);
                    await newValve.ActivateAsync(tagToActivate);
                    await BindChildrenAsync(newValve, tagToActivate);
                    resultList.Add(newValve);
                }
                else
                {
                    // otherwise, create a broadcast valve
                    var newValve = new Valve2(fixture);
                    resultList.Add(newValve);
                }
            }

            return resultList.ToArray();
        }

        async Task BindChildrenAsync(StateValve2 parentValve, int tagToActivate)
        {
            Valve2[] childValves = null;

            Action disposeChildValve = () =>
                {
                    foreach (var cvalve in childValves)
                        cvalve.Dispose();
                };

            Func<int, Task<Valve2[]>> recursiveBindAsync = async ta =>
                {
                    var childFixtures = parentValve.Fixture.CreateChildFixtures();
                    return await BindAsync(childFixtures, ta);
                };

            childValves = await recursiveBindAsync(tagToActivate);

            parentValve.Updated += async (sender, tag) => 
                {
                    disposeChildValve();
                    childValves = await recursiveBindAsync(tag);
                };
            parentValve.Disposing += (s, e) => disposeChildValve();
        }
            
        class Bindings : IBindings
        {

            Valve2[] _valves;

            public Bindings(Valve2[] valves)
            {
                _valves = valves;
            }

            public int Count { get { return _valves.Length; } }

            public void Dispose()
            {
                if (_valves == null)
                    return;
                foreach (var valve in _valves)
                {
                    valve.Dispose();
                }
                _valves = null;
            }

        }
    }
}
