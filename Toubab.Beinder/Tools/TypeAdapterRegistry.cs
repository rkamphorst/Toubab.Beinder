using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Toubab.Beinder.Tools
{
    public class TypeAdapterRegistry<IAdapter>
        where IAdapter : class
    {
        readonly TypeInfo _baseAdapterTypeInfo = typeof(IAdapter).GetTypeInfo();
        readonly LinkedList<KeyValuePair<TypeInfo, List<Type>>> _typeRegistry
            = new LinkedList<KeyValuePair<TypeInfo, List<Type>>>();

        public void RegisterFromAssembly(Assembly assembly)
        {
            foreach (var pair in assembly.ExportedTypes
                .Where(t => _baseAdapterTypeInfo.IsAssignableFrom(t.GetTypeInfo()))
                .SelectMany(t => 
                    GetAdapteeTypes(t).Select(adapteeType => new {AdapterType = t,AdapteeType = adapteeType })
                ))
                RegisterInternal(pair.AdapteeType, pair.AdapterType);
        }

        public void Register<T>() 
            where T : IAdapter
        {
            Register(typeof(T));
        }

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

        public IEnumerable<Type> FindAdapterTypesFor<TAdaptee>(TAdaptee adaptee = default(TAdaptee))
        {
            return FindAdapterTypesFor(typeof(TAdaptee));
        }

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

        protected virtual IEnumerable<Type> GetAdapteeTypes(Type adapterType)
        {
            return adapterType.EnumerateGenericAdapteeArguments<IAdapter>();
        }
            
    }

}
