using System;
using System.Collections.Generic;
using System.Linq;

namespace Toubab.Beinder.PropertyScanners
{
    public class DefaultPropertyScanner : CombinedPropertyScanner
    {
        public DefaultPropertyScanner() 
        {
            Add(new TypeExtensionsScanner(this));
            Add(new CustomPropertyScanner());
            Add(new NotifyPropertyChangedPropertyScanner());
            Add(new ReflectionPropertyScanner());
        }
    }
    
}
