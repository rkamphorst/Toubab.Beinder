namespace Toubab.Beinder.Valves
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Scanners;
    using Bindables;
    using Paths;

    public static class ScannerUtilities
    {
        public static List<Conduit> ScanChildConduits(this IScanner scanner, Conduit toScan)
        {
            var prop = toScan.Bindable as IProperty;
            if (prop != null)
            {
                using (toScan.Attach())
                {
                    return ScanObjectToConduits(scanner, prop.Value, toScan.AbsolutePath, toScan.Tag);
                }
            }
            return new List<Conduit>();
        }

        public static List<Conduit> ScanObjectToConduits(this IScanner scanner, object toScan, Path basePath, int tag)
        {
            if (toScan != null)
            {
                return scanner
                    .Scan(toScan)
                    .Select(b => Conduit.Create(b, toScan, basePath, tag))
                    .ToList();
            }
            return new List<Conduit>();
        }
    }
}

