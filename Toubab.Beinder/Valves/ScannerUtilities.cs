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
        public static List<Conduit> ScanForChildrenOnConduit(this IScanner scanner, Conduit toScan)
        {
            var prop = toScan.Bindable as IProperty;
            if (prop != null)
            {
                using (toScan.Attach())
                {
                    return ScanObjectAndCreateConduits(scanner, prop.Value, toScan.AbsolutePath, toScan.Family, toScan.Generation + 1);
                }
            }
            return new List<Conduit>();
        }

        public static List<Conduit> ScanObjectAndCreateConduits(this IScanner scanner, object toScan, Path basePath, int family, int generation)
        {
            if (toScan != null)
            {
                return scanner
                    .Scan(toScan)
                    .Select(b => Conduit.Create(b, toScan, basePath, family, generation))
                    .ToList();
            }
            return new List<Conduit>();
        }
    }
}

