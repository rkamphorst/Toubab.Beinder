using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Beinder
{
    public class TypeAdapterRegistry
    {
        readonly LinkedList<KeyValuePair<TypeInfo, object>> _registry 
            = new LinkedList<KeyValuePair<TypeInfo, object>>();
        readonly Dictionary<Type,object> _cache 
            = new Dictionary<Type, object>();

        readonly bool _isSingleton;
        readonly Func<Type, Type> _adapteeGetter;

        public TypeAdapterRegistry(bool singletonAdapter, Func<Type, Type> adapteeGetter)
        {
            _adapteeGetter = adapteeGetter;
            _isSingleton = singletonAdapter;
        }

        Type GetAdaptee(Type adapterType)
        {
            var adaptee = _adapteeGetter(adapterType);
            if (adaptee == null)
                return null;

            return 
                adaptee != null &&
            adapterType.GetTypeInfo().DeclaredConstructors.Any(c => c.GetParameters().Length == 0)
                    ? adaptee
                    : null;
        }

        public void RegisterFromAssembly(Assembly assembly)
        {
            foreach (var pair in assembly.ExportedTypes
                .Select(t => new {AdapterType = t,AdapteeType = GetAdaptee(t)})
                .Where(x => x.AdapteeType != null))
                RegisterInternal(pair.AdapteeType, pair.AdapterType);
        }

        public void Register(Type adapterType)
        {
            var adapteeType = GetAdaptee(adapterType);
            if (adapteeType == null)
                throw new ArgumentException();
            
            RegisterInternal(adapteeType, adapterType);
        }

        void RegisterInternal(Type adapteeType, Type adapterType)
        {
            var adapteeTypeInfo = adapteeType.GetTypeInfo();
            var adapter = _isSingleton ? adapterType : Activator.CreateInstance(adapterType);
            var newPair = new KeyValuePair<TypeInfo, object>(adapteeTypeInfo, adapter);

            _cache[adapteeType] = newPair.Value;
            LinkedListNode<KeyValuePair<TypeInfo, object>> node = _registry.First;
            while (node != null)
            {
                if (node.Value.Key.Equals(adapteeTypeInfo))
                {
                    _registry.AddBefore(node, newPair);
                    _registry.Remove(node);
                }
                else if (node.Value.Key
                    .IsAssignableFrom(adapteeTypeInfo))
                {
                    _registry.AddBefore(node, newPair);
                }
            }
            _registry.AddLast(newPair);
        }

        public object Resolve(Type adapteeType)
        {
            object adapter;
            if (!_cache.TryGetValue(adapteeType, out adapter))
            {
                adapter = _registry
                    .Where(pair => pair.Key.IsAssignableFrom(adapteeType.GetTypeInfo()))
                    .Select(pair => pair.Value)
                    .FirstOrDefault();
                if (_isSingleton)
                {
                    var adapterType = adapter as Type;
                    adapter = Activator.CreateInstance(adapterType);
                }
            }
            return adapter;
        }

    }

}
