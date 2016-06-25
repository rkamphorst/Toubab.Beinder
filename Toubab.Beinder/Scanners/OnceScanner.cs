namespace Toubab.Beinder.Scanners
{
    using System;
    using System.Collections.Generic;    
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Bindables;

    public class OnceScanner : IScopedScanner
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

        public IScopedScanner NewScope() 
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

        bool IsScanned(object obj)
        {
            object isScanned;
            if (_scannedObjects.TryGetValue(obj, out isScanned))
                return true;
            if (_parent != null)
                return _parent.IsScanned(obj);
            return false;
        }

        bool PrepareForScan(object obj)
        {
            // never scan null objects
            if (obj == null) 
                return false;

            var typeinfo = obj.GetType().GetTypeInfo();

            // never scan primitive types (bool, char, int, ...) and enums
            if (typeinfo.IsPrimitive || typeinfo.IsEnum)
                return false;

            // always scan other value types and strings.
            // no need to add them to cache, as we will never encounter
            // the same reference twice (value types are passed by 
            // reference, strings are immutable and might be interned)
            if (typeinfo.IsValueType || obj is string) 
                return true;

            // don't scan objects we scanned before
            if (IsScanned(obj)) 
                return false;

            // ok, let's scan the object. first, add it to cache
            _scannedObjects.Add(obj, true);

            // ...and tell caller that it's OK to scan the object
            return true;
        }

    }
}

