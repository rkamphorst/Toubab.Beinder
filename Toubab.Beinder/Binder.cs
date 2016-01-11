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

        BroadcastValve[] Bind(object[] objects, object activator, BinderState externalState)
        {
            var resultList = new List<BroadcastValve>();

            // entryList: list of Binder Entries, each holding an IProperty instance.
            // The list is sorted on Path, which is important because
            // the shortest property path will be on top, followed by the
            // property paths that start with that path, and so on.
            // Furthermore, all properties with the same paths will be next
            // to each other in the list.
            var state = BinderState.FromScan(Scanner, objects);

            if (externalState != null)
                state.Merge(externalState);

            ValveParameters valveParams;
            while (state.PopValveParameters(out valveParams))
            {
                if (valveParams.BindableStates != null)
                {
                    var newValve = new StateValve();
                    foreach (var entry in valveParams.BindableStates)
                    {
                        var newState = (IBindableState)entry.Bindable.CloneWithoutObject();
                        newState.SetObject(entry.Object);
                        newValve.Add(newState);
                    }
                    newValve.Activate(activator);

                    BindChildValves(newValve, activator, valveParams.ExternalState);

                    resultList.Add(newValve);
                }
                if (valveParams.BindableBroadcasts != null)
                {
                    var newValve = new BroadcastValve();
                    foreach (var entry in valveParams.BindableBroadcasts)
                    {
                        var newBroadcast = entry.Bindable.CloneWithoutObject();
                        newBroadcast.SetObject(entry.Object);
                        newValve.Add(newBroadcast);
                    }

                    resultList.Add(newValve);
                }
            }

            return resultList.ToArray();
        }

        void BindChildValves(StateValve parentValve, object parentActivator, BinderState externalState)
        {
            var childValves = Bind(
                                  parentValve.GetValues(), 
                                  GetChildActivator(parentActivator, parentValve, externalState), 
                                  externalState
                              );
            parentValve.ValueChanged += (source, evt) =>
            {
                foreach (var cvalve in childValves)
                    cvalve.Dispose();
                childValves = Bind(parentValve.GetValues(), GetChildActivator(evt.Source.Object, parentValve, externalState), externalState);
            };
            parentValve.Disposing += delegate
            {
                foreach (var cvalve in childValves)
                    cvalve.Dispose();
            };
        }

        object GetChildActivator(object parentActivator, StateValve parentValve, BinderState externalState)
        {
            var activator = parentValve.GetValueForObject(parentActivator);
            if (activator == null && externalState.ContainsPropertyForObject(parentActivator))
            {
                return parentActivator;
            } 
            return activator;
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

            public ValveParameters(IEnumerable<CandidateBindable> states, IEnumerable<CandidateBindable> broadcasts, BinderState relativeProperties)
            {
                BindableStates = states;
                BindableBroadcasts = broadcasts;
                ExternalState = relativeProperties;
            }

            public IEnumerable<CandidateBindable> BindableBroadcasts { get; private set; }

            public IEnumerable<CandidateBindable> BindableStates { get; private set; }

            public BinderState ExternalState { get; private set; }
        }

        class BinderState
        {
            public static BinderState FromScan(IScanner scanner, IEnumerable<object> objects)
            {
                return new BinderState(
                    objects
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

                var states = new LinkedList<CandidateBindable>();
                var broadcasts = new LinkedList<CandidateBindable>();
                int numBcConsumers = 0;
                int numBcProducers = 0;
                var first = _list.First;
                while (first != null && first.Value.RelativePath.CompareTo(firstPath) == 0)
                {
                    _list.RemoveFirst();
                    if (first.Value.Bindable is IBindableState)
                    {
                        states.AddLast(first);
                    }
                    else
                    {
                        if (first.Value.Bindable is IBindableBroadcastConsumer)
                            numBcConsumers++;
                        else if (first.Value.Bindable is IBindableBroadcastProducer)
                            numBcProducers++;
                        broadcasts.AddLast(first);
                    }
                    first = _list.First;
                }

                var relativeBindables = new BinderState();
                if (states.Count >= 1)
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

                if (states.Count < 2 && relativeBindables._list.Count == 0)
                {
                    states = null;
                }

                if (numBcConsumers == 0 || numBcProducers == 0)
                {
                    broadcasts = null;
                }

                if (_list.Count > 0 && states == null && broadcasts == null)
                {
                    return PopValveParameters(out result);
                }
                result = new ValveParameters(states, broadcasts, relativeBindables);

                return states != null || broadcasts != null;
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
