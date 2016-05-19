namespace Toubab.Beinder.Scanners
{
    using System;
    using System.Collections.Generic;    
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Bindables;

    public class OnceScanner : IScanner
    {
        readonly ConditionalWeakTable<object,object> _scannedObjects;
        readonly IScanner _scanner;
        readonly OnceScanner _parent;

        public static OnceScanner Decorate(IScanner scanner) 
        {
            return (scanner as OnceScanner) ?? new OnceScanner(scanner);
        }

        OnceScanner(IScanner scanner)
        {
            _scanner = scanner;
            _scannedObjects = new ConditionalWeakTable<object, object>();
        }

        OnceScanner(OnceScanner parent)
            : this(parent._scanner)
        {
            _parent = parent;
        }

        public OnceScanner NewScope() 
        {
            return new OnceScanner(this);
        }

        #region IScanner implementation

        public IEnumerable<IBindable> Scan(object obj)
        {
            return PrepareForScan(obj) 
                ? _scanner.Scan(obj) 
                : Enumerable.Empty<IBindable>();
        }

        #endregion

        bool WasScanned(object obj)
        {
            object isScanned;
            if (_scannedObjects.TryGetValue(obj, out isScanned))
                return true;
            if (_parent != null)
                return _parent.WasScanned(obj);
            return false;
        }

        bool PrepareForScan(object obj)
        {
            // never scan null objects
            if (obj == null) 
                return false;

            // always scan value types and strings.
            // no need to add them to cache.
            if (obj.GetType().GetTypeInfo().IsValueType || obj is string) 
                return true;

            // don't scan objects we scanned before
            if (WasScanned(obj)) 
                return false;

            // ok, let's scan the object. first, add it to cache
            _scannedObjects.Add(obj, true);

            // ...and tell caller that it's OK to scan the object
            return true;
        }

    }
}

