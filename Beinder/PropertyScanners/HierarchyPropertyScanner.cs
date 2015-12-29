using System.Collections.Generic;
using System;
using System.Linq;
using Beinder.PropertyPathParsers;

namespace Beinder.PropertyScanners
{
    /// <summary>
    /// Base class for scanners of (view) hierarchies.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A view is generally a as a hierarchy of views. 
    /// This class is to facilitate extraction of properties from hierarchically
    /// built views (or other structures).
    /// </para>
    /// </remarks>
    public abstract class HierarchyPropertyScanner<TParent, TNode> : IObjectPropertyScanner
        where TNode : class
        where TParent : class, TNode
    {

        IPropertyPathParser _pathParser = new CamelCasePropertyPathParser();

        public IPropertyPathParser PathParser
        { 
            get { return _pathParser; }
            set { _pathParser = value; }
        }

        public IEnumerable<IProperty> Scan(object obj)
        {
            if (!(obj is TParent) || obj == null)
                return Enumerable.Empty<IProperty>();

            var tparent = (TParent) obj;

            return ScanHierarchy(tparent).Select(
                kvp => new ObjectProperty(kvp.Key, kvp.Value, _pathParser)
            );
        }
            
        IEnumerable<KeyValuePair<string,TNode>> ScanHierarchy(TParent root)
        {
            foreach (var kvp in GetChildren(root))
            {
                var parent = kvp.Value as TParent;
                if (parent != null)
                {
                    foreach (var grandchild in ScanHierarchy(parent))
                        yield return grandchild;
                }
                if (!string.IsNullOrEmpty(kvp.Key))
                    yield return kvp;
            }
        }

        /// <summary>
        /// Get the children of a parent in the hierarchy.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method should return a key-value pair, the key being a property
        /// name (possibly empty or null) and the value the object that can be
        /// potentially bound to.
        /// 
        /// All objects in the hierarchy should be returned, in order to reach 
        /// the objects that have no children (the leaves). If no property name
        /// can be derived for an object, set it to null.
        /// </remarks>
        /// <returns>Key-value pairs, keys te property names, values the objects that are
        /// the children of <paramref name="parent"/></returns>
        /// <param name="parent">Parent node.</param>
        protected abstract IEnumerable<KeyValuePair<string,TNode>> GetChildren(TParent parent);


        class ObjectProperty : IProperty
        {
            readonly PropertyPath _propertyPath;
            readonly TNode _value;
            TParent _object;

            public ObjectProperty(string key, TNode value, IPropertyPathParser pathParser)
            {
                _propertyPath = pathParser.Parse(key);
                _value = value;
            }

            private ObjectProperty(PropertyPath path, TNode value)
            {
                _propertyPath = path;
                _value = value;
            }


            public event EventHandler<PropertyValueChangedEventArgs> ValueChanged;

            public object Value
            {
                get
                {
                    return _value;
                }
            }

            public bool TrySetValue(object value)
            { 
                return false;
            }

            public object Object
            { 
                get
                {
                    return _object;
                } 
            }

            public void SetObject(object obj)
            {
                _object = (TParent) obj;
            }

            public PropertyPath Path
            {
                get
                {
                    return _propertyPath;
                }
            }

            public IProperty CloneWithoutObject()
            {
                return new ObjectProperty(_propertyPath, _value);
            }

        }

    }

}
