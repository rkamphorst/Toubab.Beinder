using System.Collections.Generic;
using System.Linq;
using Toubab.Beinder.Scanner;
using System;
using System.Collections;
using Toubab.Beinder.Valve;

namespace Toubab.Beinder
{

    public class Binder
    {
        public static CombinedScanner CreateDefaultPropertyScanner()
        {
            var result = new CombinedScanner();
            result.Add(new ReflectionScanner());
            result.Add(new NotifyPropertyChangedScanner());
            result.Add(new DictionaryScanner());
            result.Add(new TypeExtensionsScanner(result));
            result.Add(new CustomBindableScanner());
            return result;
        }

        readonly CombinedScanner _propertyScanner;

        public Binder()
            : this(CreateDefaultPropertyScanner())
        {
        }

        public Binder(CombinedScanner propertyScanner)
        {
            _propertyScanner = propertyScanner;

        }

        public Binder(params IScanner[] bindableScanners)
        {
            var combinedScanner = new CombinedScanner();
            foreach (var ps in bindableScanners)
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

        public IBindings Bind(params object[] objects)
        {
            return Bind((IEnumerable<object>)objects);
        }

        public IBindings Bind(IEnumerable<object> objects)
        {
            var objectArray = objects.ToArray();
            var activator = objectArray.FirstOrDefault();
            return new Bindings(Bind(objectArray, activator, null));
        }

        BroadcastValve[] BindMultiple(object[][] objectss, object[] activators, object parentActivator, BinderState externalState)
        {
            var results = new List<BroadcastValve[]>();
            for (int i = 0; i < objectss.Length; i++)
            {
                var activator = (activators.Length > i ? activators[i] : parentActivator) ?? parentActivator;
                results.Add(Bind(objectss[i], activator, externalState));
            }
            return results.SelectMany(r => r).ToArray();
        }

        BroadcastValve[] Bind(object[] objects, object activator, BinderState externalState)
        {
            var resultList = new List<BroadcastValve>();

            var state = BinderState.FromScan(Scanner, objects);
            if (externalState != null)
                state.Merge(externalState);

            ValveParameters valveParams;
            while (state.PopValveParameters(out valveParams))
            {
                BroadcastValve newValve;

                var bs = new List<IBindable>();
                foreach (var c in valveParams.Bindables)
                {
                    var b = c.Bindable.CloneWithoutObject();
                    b.SetObject(c.Object);
                    bs.Add(b);
                }

                if (valveParams.ContainsState)
                {
                    var v = new StateValve();
                    foreach (var b in bs)
                        v.Add(b);
                    v.Activate(activator);
                    BindChildValves(v, activator, valveParams.ExternalState);
                    newValve = v;
                }
                else
                {
                    var v = new BroadcastValve();
                    foreach (var b in bs)
                        v.Add(b);
                    newValve = v;
                }

                resultList.Add(newValve);
            }

            return resultList.ToArray();
        }

        void BindChildValves(StateValve parentValve, object parentActivator, BinderState externalState)
        {
            BroadcastValve[] childValves = null;

            Action disposeChildValve = () =>
            {
                foreach (var cvalve in childValves)
                    cvalve.Dispose();
            };

            Func<object, BroadcastValve[]> recursiveBind = (pa) =>
            {
                var childObjects = parentValve.GetChildValveObjects();
                var activators = parentValve.GetValueForObject(pa);
                return BindMultiple(childObjects, activators, pa, externalState);
            };

            childValves = recursiveBind(parentActivator);
            
            parentValve.ValueChanged += (s, e) =>
            {
                disposeChildValve();
                childValves = recursiveBind(e.Source.Object);
            };
            parentValve.Disposing += (s, e) => disposeChildValve();
        }

        struct CandidateBindable
        {
            public CandidateBindable(object obj, IBindable property)
                : this(obj, property, new Path(property.Path))
            {
            }

            CandidateBindable(object obj, IBindable bindable, Path path)
            {
                Object = obj;
                Bindable = bindable;
                RelativePath = path;
            }

            public IBindable Bindable { get; private set; }

            public Path RelativePath { get; private set; }

            public object Object { get; private set; }

            public CandidateBindable? RelativeTo(Path basePath)
            {
                var newPath = RelativePath.RelativeTo(basePath);
                return newPath != null ? (CandidateBindable?)new CandidateBindable(Object, Bindable, newPath) : null;
            }

            public override string ToString()
            {
                return string.Format("[BinderEntry: Path={0}, Object={1}]", RelativePath, Object);
            }
        }

        struct ValveParameters
        {

            public ValveParameters(IEnumerable<CandidateBindable> bindables, bool containsState, BinderState relativeProperties)
            {
                Bindables = bindables;
                ExternalState = relativeProperties;
                ContainsState = containsState;
            }

            public bool ContainsState { get; set; }

            public IEnumerable<CandidateBindable> Bindables { get; private set; }

            public BinderState ExternalState { get; private set; }
        }

        class BinderState
        {
            public static BinderState FromScan(IScanner scanner, IEnumerable<object> objects)
            {
                return new BinderState(
                    objects
                    .Where(o => o != null)
                    .SelectMany(o => scanner.Scan(o).Select(p => new CandidateBindable(o, p)))
                    .OrderBy(be => be.RelativePath)
                );
            }

            readonly LinkedList<CandidateBindable> _list;

            BinderState()
            {
                _list = new LinkedList<CandidateBindable>();
            }

            BinderState(IEnumerable<CandidateBindable> collection)
            {
                _list = new LinkedList<CandidateBindable>(collection);
            }

            public bool PopValveParameters(out ValveParameters result)
            {
                var firstPath = _list.Count == 0 ? null : _list.First.Value.RelativePath;

                var bindables = new LinkedList<CandidateBindable>();
                int numConsumers = 0;
                int numProducers = 0;
                int numStates = 0;
                var first = _list.First;
                while (first != null && first.Value.RelativePath.CompareTo(firstPath) == 0)
                {
                    _list.RemoveFirst();
                    if (first.Value.Bindable is IBindableBroadcastConsumer)
                        numConsumers++;
                    if (first.Value.Bindable is IBindableBroadcastProducer)
                        numProducers++;
                    if (first.Value.Bindable is IBindableState)
                        numStates++;
                    bindables.AddLast(first);
                    first = _list.First;
                }

                var relativeBindables = new BinderState();
                if (numStates >= 1)
                {
                    while (_list.Count > 0)
                    {
                        var rebased = _list.First.Value.RelativeTo(firstPath);
                        if (!rebased.HasValue)
                            break;

                        relativeBindables._list.AddLast(rebased.Value);
                        _list.RemoveFirst();
                    }
                }

                if (bindables.Count < 2 && relativeBindables._list.Count == 0)
                {
                    bindables = null;
                }

                if (_list.Count > 0 && bindables == null)
                {
                    return PopValveParameters(out result);
                }
                result = new ValveParameters(bindables, numStates >= 1, relativeBindables);

                return bindables != null;
            }

            public void Merge(BinderState toMerge)
            {
                var curState = _list.First;

                foreach (var mergeEntry in toMerge._list)
                {
                    while (curState != null)
                    {
                        if (curState.Value.RelativePath.CompareTo(mergeEntry.RelativePath) > 0)
                        {
                            // cur comes *after* mergeEntry, so add newProp before cur
                            _list.AddBefore(curState, mergeEntry);
                            break;
                        }
                        curState = curState.Next;
                    }
                    if (curState == null)
                    {
                        // traversed the whole list without adding...
                        _list.AddLast(mergeEntry);
                    }
                }
            }

            public bool ContainsPropertyForObject(object o)
            {
                return _list.Any(p => ReferenceEquals(o, p.Object));
            }
        }

        class Bindings : IBindings
        {

            BroadcastValve[] _valves;

            public Bindings(BroadcastValve[] valves)
            {
                _valves = valves;
            }

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


            public IEnumerator<object> GetEnumerator()
            {
                return _valves.Cast<object>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

    }
}
