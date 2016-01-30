namespace Toubab.Beinder
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Bindable;
    using Scanner;
    using Valve;
    using Mixin;

    /// <summary>
    /// Binder: binds properties, events and methods of different objects
    /// </summary>
    /// <remarks>
    /// The <see cref="Binder"/> uses a <see cref="CombinedScanner"/> to harvest
    /// <see cref="IBindable"/> instances (representing properties, events and methods)
    /// from one or more objects.
    /// 
    /// The collection of <see cref="IBindable"/>s is searched for matching property names;
    /// bindables with matching property names are bound to each other. 
    /// 
    /// The principal entry point(s) for this class are <see cref="Bind(object[])"/> and 
    /// <see cref="Bind(IEnumerable{object})"/>.
    /// 
    /// All private methods are also documented here, because the code is quite complex and 
    /// of vital importance to Toubab.Beinder.
    /// </remarks>
    public class Binder
    {
        /// <summary>
        /// Creates the default combined scanner that combines the standard scanners.
        /// </summary>
        public static CombinedScanner CreateDefaultCombinedScanner()
        {
            var result = new CombinedScanner();
            result.Add(new ReflectionScanner());
            result.Add(new NotifyPropertyChangedScanner());
            result.Add(new DictionaryScanner());

            var mixinScanner = new MixinScanner(result);
            mixinScanner.AdapterRegistry.Register<CommandMixin>();
            result.Add(mixinScanner);
            
            result.Add(new CustomBindableScanner());
            return result;
        }

        readonly CombinedScanner _propertyScanner;

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <remarks>
        /// Initializes the <see cref="Binder"/> with the default combined scanner
        /// (See <see cref="CreateDefaultCombinedScanner()"/>).
        /// </remarks>
        public Binder()
            : this(CreateDefaultCombinedScanner())
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// Initializes the <see cref="Binder"/> with given combined scanner.
        /// </remarks>
        public Binder(CombinedScanner combinedScanner)
        {
            _propertyScanner = combinedScanner;

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// Initializes the <see cref="Binder"/> with a combined scanner that 
        /// combines given <paramref name="scanners"/>.
        /// </remarks>
        public Binder(params IScanner[] scanners)
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

        /// <summary>
        /// The (combined) scanner used by this <see cref="Binder"/> instance.
        /// </summary>
        public CombinedScanner Scanner { get { return _propertyScanner; } }

        /// <summary>
        /// Bind properties of given objects.
        /// </summary>
        /// <remarks>
        /// This is a utility method that merely calls <see cref="Bind(IEnumerable{object})"/>.
        /// </remarks>
        public IBindings Bind(params object[] objects)
        {
            return Bind((IEnumerable<object>)objects);
        }

        /// <summary>
        /// Bind properties of given objects.
        /// </summary>
        /// <remarks>
        /// The <see cref="Binder"/> uses a <see cref="Scanner" /> to harvest
        /// <see cref="IBindable"/> instances (representing properties, events and methods)
        /// from <paramref name="objects"/>.
        /// 
        /// The collection of <see cref="IBindable"/>s is searched for matching bindable names;
        /// bindables with matching bindable names are bound to each other. 
        /// 
        /// This is done recursively, so that properties of properties are also bound (this 
        /// can only be the case for bindable properties, represented with
        /// <see cref="IProperty"/>). Furthermore, when a property's value changes, 
        /// the "properties of properties" (child properties) are automatically rebound.
        /// 
        /// When the binding is established, values of properties from the first object
        /// are propagated to the bound properties on the other objects.
        /// </remarks>
        public IBindings Bind(IEnumerable<object> objects)
        {
            var objectArray = objects.ToArray();
            var activator = objectArray.FirstOrDefault();
            return new Bindings(Bind(objectArray, activator, null));
        }

        /// <summary>
        /// Bind properties of given objects.
        /// </summary>
        /// <remarks>
        /// The <paramref name="activator"/> is the object of which the property values are
        /// propagated to the bound property values.
        /// 
        /// The <paramref name="externalState"/> contains a list of <see cref="IBindable"/> 
        /// instances that "cross object boundaries": they were defined on a parent object, 
        /// but need to be bound to a <see cref="IBindable"/> of child (or descendant) object.
        /// 
        /// This method is called by <see cref="Bind(IEnumerable{object})"/> to do the
        /// initial binding, and by <see cref="BindChildren"/> via <see cref="BindMultiple"/>
        /// to do the recursive (re)binding.
        /// </remarks>
        BroadcastValve[] Bind(object[] objects, object activator, BinderState externalState)
        {
            var resultList = new List<BroadcastValve>();

            // use Scanner to can given objects for IBindable objects
            var state = BinderState.FromScan(Scanner, objects);

            // merge external state into state. 
            // state and external state both contain a sorted list (on Path) 
            // of IBindables; externalState is merged in such a way that 
            // state's list remains sorted on Path.
            if (externalState != null)
                state.Merge(externalState);

            ValveParameters valveParams;
            while (state.PopValveParameters(out valveParams))
            {
                // valveParams now contains 2 properties:
                // - Bindables, the bindables that will participate in a new Valve
                // - ExternalState, external state for the recursive binding 
                //                  (state that needs to be bound to child properties)
                BroadcastValve newValve;

                // create a list of attached bindables
                // a bindable is attached if an object has been assigned to it.
                // Note: the actual bindables are clones of the ones
                // that are in valveParams.Bindables.
                var bs = new List<IBindable>();
                foreach (var c in valveParams.Bindables)
                {
                    var b = (IBindable) c.Bindable.CloneWithoutObject();
                    b.SetObject(c.Object);
                    bs.Add(b);
                }
                    
                if (valveParams.ContainsState)
                {
                    // if any of the bindables is a state bindable,
                    // all bindables go into a state valve
                    var v = new StateValve();
                    foreach (var b in bs)
                        v.Add(b);

                    // only state valves need activation
                    v.Activate(activator);
                    // .. and binding of child properties
                    BindChildren(v, activator, valveParams.ExternalState);
                    newValve = v;
                }
                else
                {
                    // otherwise, create a broadcast valve
                    var v = new BroadcastValve();
                    foreach (var b in bs)
                        v.Add(b);
                    newValve = v;
                }

                resultList.Add(newValve);
            }

            return resultList.ToArray();
        }

        /// <summary>
        /// Bind the children of <paramref name="parentValve"/>.
        /// </summary>
        /// <remarks>
        /// Given a valve that binds matching properties from objects, also bind the properties
        /// of the properties (the "children") in child valves.
        /// 
        /// See the <see cref="Bind(object[], object, BinderState)"/> for more details on how this
        /// method is used.
        /// </remarks>
        /// <param name="parentValve">Parent valve.</param>
        /// <param name="parentActivator">Parent activator object.</param>
        /// <param name="externalState">External state</param>
        void BindChildren(StateValve parentValve, object parentActivator, BinderState externalState)
        {
            BroadcastValve[] childValves = null;

            Action disposeChildValve = () =>
            {
                foreach (var cvalve in childValves)
                    cvalve.Dispose();
            };

            Func<object, BroadcastValve[]> recursiveBind = pa =>
            {
                var childObjects = parentValve.GetChildValveObjects();
                var activators = parentValve.GetValuesForObject(pa);
                return BindMultiple(childObjects, activators, pa, externalState);
            };

            childValves = recursiveBind(parentActivator);
            
            parentValve.ValueChanged += (s, e) =>
            {
                disposeChildValve();
                childValves = recursiveBind(e.SourceObject);
            };
            parentValve.Disposing += (s, e) => disposeChildValve();
        }

        /// <summary>
        /// Bind multiple arrays of objects 
        /// </summary>
        /// <remarks>
        /// <see cref="BindChildren"/> binds matching properties / bindables of child properties.
        /// 
        /// Because every instance of <see cref="IBindable"/> (and notably, <see cref="IProperty"/>)
        /// advertises multiple values to bind, this results in *multiple* valves for each
        /// property. <see cref="BindMultiple"/> is a helper method that takes care
        /// of iterating over each set of parameters, calling <see cref="Bind(object[], object, BinderState)"/> 
        /// and concatenating all results into a single array.
        /// 
        /// Note that <paramref name="parentActivator"/> is also passed as a parameter. This object
        /// is substituted where <paramref name="activators"/> contains <c>null</c>, or where
        /// activators does not provide a suitable activator because it does not contain enough
        /// values.
        /// 
        /// Example.
        /// 
        /// <paramref name="objectss"/> (may contain <c>null</c> entries):
        /// 
        ///     [ [a,b,c], [d,e,f,g], [h,i] ]
        /// 
        /// <paramref name="activators"/> (may also contain <c>null</c> entries):
        /// 
        ///     [ c, f, null ]
        /// 
        /// <paramref name="parentActivator"/>; <paramref name="externalState"/>:
        /// 
        ///     p; x
        /// 
        /// <see cref="Bind(object[], object, BinderState)"/> is then called three times
        /// with the following parameters:
        /// 
        ///    ( [a,b,c]  , c,  x )
        ///    ( [d,e,f,g], f,  x )
        ///    ( [h,i]    , p,  x )
        /// 
        /// All resulting arrays of BroadcastValves (StateValve is a subclass of BroadcastValve)
        /// are concatenated and returned in an array.
        /// 
        /// </remarks>
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

        /// <summary>
        /// Binder state: an ordered list of <see cref="IBindable" /> instances
        /// </summary>
        /// <remarks>
        /// BinderState is used to encapsulate the state that is passed on from binding of parent
        /// valves to binding of child valves, guaranteeing that the list remains
        /// ordered and removing a lot of clutter from <see cref="Bind(IEnumerable{object})"/>.
        /// </remarks>
        class BinderState
        {
            /// <summary>
            /// Create a new instance from a scan of given objects
            /// </summary>
            /// <returns><see cref="BinderState"/> with <see cref="IBindable"/> instances that 
            /// were harvested from <paramref name="objects"/>.</returns>
            /// <param name="scanner">Scanner used to harvest bindables from <paramref name="objects"/>.</param>
            /// <param name="objects">Objects to harvest bindables from.</param>
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

            /// <summary>
            /// Get the next set of parameters with which a valve (either <see cref="StateValve"/>
            /// or <see cref="BroadcastValve"/>) can be instantiated.
            /// </summary>
            /// <remarks>
            /// The parameters are the following:
            /// 
            /// - *Bindables*: A list (<see cref="IEnumerable"/>) of <see cref="IBindable"/>  instances 
            ///   that all have the same path. 
            /// - *ContainsState*: Whether any of the bindables in this list are *state bindables*.
            /// - *ExternalState*: If any of the bindables are state bindables, we need to consider 
            ///   child valves. If child valves are created, we may need to create "external state". 
            ///   External state consists of all those bindables that have a path that starts with the 
            ///   path of the bindables from the previous list, and should be bound to properties, events 
            ///   or methods (bindables) on the values of the bindables in the previous list.
            /// 
            /// Because the list in <see cref="BinderState"/> is sorted, the *Bindables* consist
            /// of all the first elements in the list that have the same path. If any of those 
            /// is a <see cref="IProperty"/> ("state bindable"), *ContainsState* is true.
            /// 
            /// If *ContainsState* is true, the *ExternalState* consists of all those states that have 
            /// a path that starts with the first path.
            /// 
            /// If there is only one bindable with a path and it is not a state bindable, there is
            /// no point in creating a valve for it; it is skipped by <see cref="PopValveParameters"/>.
            /// 
            /// If there are no more parameters to pop, this method returns <c>false</c>; 
            /// <paramref name="result"/> will be <c>default(ValveParameters)</c>.
            /// </remarks>
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
                    if (first.Value.Bindable is IEventHandler)
                        numConsumers++;
                    if (first.Value.Bindable is IEvent)
                        numProducers++;
                    if (first.Value.Bindable is IProperty)
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
                result = bindables == null ? default(ValveParameters) : new ValveParameters(bindables, numStates >= 1, relativeBindables);

                return bindables != null;
            }

            /// <summary>
            /// Merge another binder state into this one, preserving order
            /// </summary>
            /// <remarks>
            /// Needed to merge external state (<see cref="ValveParameters.ExternalState"/>)
            /// into state from a scan (<see cref="BinderState.FromScan"/>).
            /// </remarks>
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

        }

        /// <summary>
        /// A "candidate" bindable
        /// </summary>
        /// <remarks>
        /// This is a utility class to simplify passing on bindables that should be
        /// "attached" later on ("attach" as in "attach an object to it with 
        /// <see cref="IMixin.SetObject(object)"/>). The relative path is 
        /// needed separately (next to <see cref="IBindable.Path"/>) for the
        /// cases where the candidate bindable is bound in a child context, i.e.,
        /// is passed to <see cref="BindChildren"/> through the <c>externalState</c>
        /// parameter.
        /// </remarks>
        struct CandidateBindable
        {
            public CandidateBindable(object obj, IBindable property)
                : this(obj, property, new Path.Path(property.Path))
            {
            }

            CandidateBindable(object obj, IBindable bindable, Path.Path path)
            {
                Object = obj;
                Bindable = bindable;
                RelativePath = path;
            }

            /// <summary>
            /// The bindable with no object attached.
            /// </summary>
            public IBindable Bindable { get; private set; }

            /// <summary>
            /// Relative path. 
            /// </summary>
            /// <remarks>
            /// If passed to <see cref="BindChildren"/> in the <c>externalState</c>
            /// parameter, this path is relative to the objects in the <c>objects</c>
            /// parameter.
            /// </remarks>
            public Path.Path RelativePath { get; private set; }

            /// <summary>
            /// The object to be attached to <see cref="Bindable"/>.
            /// </summary>
            public object Object { get; private set; }

            /// <summary>
            /// Creates a new bindable relative to a base path.
            /// </summary>
            /// <remarks>
            /// Returns <c>null</c> (empty nullable) if <see cref="RelativePath"/>
            /// does not start with <paramref name="basePath"/>.
            /// </remarks>
            public CandidateBindable? RelativeTo(Path.Path basePath)
            {
                var newPath = RelativePath.RelativeTo(basePath);
                return newPath != null ? (CandidateBindable?)new CandidateBindable(Object, Bindable, newPath) : null;
            }

            public override string ToString()
            {
                return string.Format("[BinderEntry: Path={0}, Object={1}]", RelativePath, Object);
            }
        }

        /// <summary>
        /// Parameters to create a valve with
        /// </summary>
        /// <remarks>
        /// For further explanation of these parameters, see <see cref="BinderState.PopValveParameters"/>.
        /// </remarks>
        struct ValveParameters
        {

            public ValveParameters(IEnumerable<CandidateBindable> bindables, bool containsState, BinderState relativeProperties)
            {
                Bindables = bindables;
                ExternalState = relativeProperties;
                ContainsState = containsState;
            }

            /// <summary>
            /// Whether any of the bindables in <see cref="Bindables"/> is a state bindable (<see cref="IProperty"/>)
            /// </summary>
            /// <remarks>
            /// If this property is true, a <see cref="StateValve"/> should be created;
            /// otherwise, a <see cref="BroadcastValve"/>.
            /// </remarks>
            public bool ContainsState { get; set; }

            /// <summary>
            /// Bindables to be grouped together into a <see cref="BroadcastValve"/> or a <see cref="StateValve"/>
            /// </summary>
            public IEnumerable<CandidateBindable> Bindables { get; private set; }

            /// <summary>
            /// External state.
            /// </summary>
            /// <remarks>
            /// Only useful if <see cref="ContainsState"/> is <c>true</c>
            /// </remarks>
            public BinderState ExternalState { get; private set; }
        }

        /// <summary>
        /// Wrapper to return bindings in
        /// </summary>
        /// <remarks>
        /// Implements the <see cref="IDisposable"/> interface, to make
        /// disposal of bindings easy.
        /// </remarks>
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


            public IEnumerator<IGrouping<Path.Path, IBindable>> GetEnumerator()
            {
                return _valves.Cast<IGrouping<Path.Path, IBindable>>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
                
        }
    }
}
