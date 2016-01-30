namespace Toubab.Beinder.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Registry for adapters that adapt to a common interface <typeparamref name="IAdapter"/>.
    /// </summary>
    public class TypeAdapterRegistry<IAdapter>
        where IAdapter : class
    {
        readonly TypeInfo _baseAdapterTypeInfo = typeof(IAdapter).GetTypeInfo();
        readonly LinkedList<KeyValuePair<TypeInfo, List<Type>>> _typeRegistry
            = new LinkedList<KeyValuePair<TypeInfo, List<Type>>>();

        /// <summary>
        /// Register all types from given assembly that are of type <typeparamref name="IAdapter"/>.
        /// </summary>
        /// <param name="assembly">Assembly to get types from</param>
        public void RegisterFromAssembly(Assembly assembly)
        {
            foreach (var pair in assembly.ExportedTypes
                .Where(t => _baseAdapterTypeInfo.IsAssignableFrom(t.GetTypeInfo()))
                .SelectMany(t => 
                    GetAdapteeTypes(t).Select(adapteeType => new {AdapterType = t,AdapteeType = adapteeType })
                ))
                RegisterInternal(pair.AdapteeType, pair.AdapterType);
        }

        /// <summary>
        /// Register type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to register</typeparam>
        public void Register<T>() 
            where T : IAdapter
        {
            Register(typeof(T));
        }

        /// <summary>
        /// Register type <paramref name="adapterType"/>
        /// </summary>
        /// <param name="adapterType">Tye type to register</param>
        public void Register(Type adapterType)
        {
            if (!_baseAdapterTypeInfo.IsAssignableFrom(adapterType.GetTypeInfo()))
                throw new ArgumentException(
                    "Given type is not assignable from " + _baseAdapterTypeInfo.FullName, 
                    "adapterType"
                );
            
            var adapteeTypes = GetAdapteeTypes(adapterType);

            bool registered = false;
            foreach (var adapteeType in adapteeTypes)
            {
                registered = true;
                RegisterInternal(adapteeType, adapterType);
            }

            if (!registered)
            {
                throw new ArgumentException(
                    "Adaptee could not be determined for " + adapterType.FullName, 
                    "adapterType"
                );
            }
        }

        void RegisterInternal(Type adapteeType, Type adapterType)
        {
            var adapteeTypeInfo = adapteeType.GetTypeInfo();
            LinkedListNode<KeyValuePair<TypeInfo, List<Type>>> node = _typeRegistry.First;
            while (node != null)
            {
                if (Equals(node.Value.Key, adapteeTypeInfo))
                {
                    node.Value.Value.Add(adapterType);
                }
                else if (node.Value.Key
                    .IsAssignableFrom(adapteeTypeInfo))
                {
                    _typeRegistry.AddBefore(node, 
                        new KeyValuePair<TypeInfo, List<Type>>(adapteeTypeInfo, new List<Type> { adapterType })
                    );
                    return;
                }
                node = node.Next;
            }
            _typeRegistry.AddLast(new KeyValuePair<TypeInfo, List<Type>>(adapteeTypeInfo, new List<Type> { adapterType }));
        }

        /// <summary>
        /// Find all types in the registry that adapt type <typeparamref name="TAdaptee"/> to
        /// <typeparamref name="IAdapter"/>.
        /// </summary>
        /// <remarks>
        /// This method *only* finds the adapters! It does not associate adaptee and adapter,
        /// this has to be done separately.
        /// </remarks>
        /// <typeparam name="TAdaptee">The type to be adapted to <typeparamref name="IAdapter"/></typeparam>
        /// <param name="adaptee">Not used; but if used, you don't have to specify <typeparamref name="TAdaptee"/>
        /// as it is inferred by the compiler from <paramref name="adaptee"/>.</param>
        public IEnumerable<Type> FindAdapterTypesFor<TAdaptee>(TAdaptee adaptee = default(TAdaptee))
        {
            return FindAdapterTypesFor(typeof(TAdaptee));
        }

        /// <summary>
        /// Find all types in the registry that adapt type <paramref name="adapteeType"/> to
        /// <typeparamref name="IAdapter"/>.
        /// </summary>
        /// <remarks>
        /// This method *only* finds the adapters! It does not associate adaptee and adapter,
        /// this has to be done separately.
        /// </remarks>
        public IEnumerable<Type> FindAdapterTypesFor(Type adapteeType)
        {
            var resolvedTypes = new HashSet<Type>();
            foreach (var pair in _typeRegistry)
            {
                if (pair.Key.IsAssignableFrom(adapteeType.GetTypeInfo()))
                {
                    for (int i = pair.Value.Count - 1; i >= 0; i--)
                    {
                        var typ = pair.Value[i];
                        if (!resolvedTypes.Contains(typ))
                        {
                            resolvedTypes.Add(typ);
                            yield return typ;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Given an adapter, determine which type(s) of adaptees it can adapt.
        /// </summary>
        /// <remarks>
        /// Used by <see cref="Register"/> to build up an adaptee-adapter search index.
        /// </remarks>
        /// <param name="adapterType">Type of the adapter.</param>
        protected virtual IEnumerable<Type> GetAdapteeTypes(Type adapterType)
        {
            return adapterType.EnumerateGenericAdapteeArguments<IAdapter>();
        }
            
    }

}
