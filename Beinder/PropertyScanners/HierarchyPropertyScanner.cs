using System.Collections.Generic;
using System;
using System.Linq;

namespace Beinder.PropertyScanners
{

    public abstract class HierarchyPropertyScanner<TParent, TNode> : IObjectPropertyScanner
        where TNode : class
        where TParent : class, TNode
    {
        protected HierarchyPropertyScanner()
        {
            AdapterRegistry = new TypeAdapterRegistry(false, GetAdapteeType);
        }

        public TypeAdapterRegistry AdapterRegistry { get; private set; }

        public virtual IEnumerable<IProperty> Scan(object obj)
        {
            var root = obj as TParent;
            if (root == null)
                Enumerable.Empty<IProperty>();

            foreach (var child in ScanHierarchy(root))
            {
                var objProp = AdapterRegistry.Resolve(child.GetType()) as IProperty;
                if (objProp != null)
                {
                    yield return objProp;
                }
            }
        }

        IEnumerable<TNode> ScanHierarchy(TParent root)
        {
            foreach (TNode child in GetChildren(root))
            {
                var parent = child as TParent;
                if (parent != null)
                {
                    foreach (var grandchild in ScanHierarchy(parent))
                        yield return grandchild;
                }
                yield return child;
            }
        }

        protected abstract IEnumerable<TNode> GetChildren(TParent parent);

        protected abstract Type GetAdapteeType(Type adapterType);
    }

}
