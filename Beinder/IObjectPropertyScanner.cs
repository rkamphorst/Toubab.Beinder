using System.Collections.Generic;
using System;

namespace Beinder
{
    
    public interface IObjectPropertyScanner
    {
        IEnumerable<IProperty> Scan(object obj);
    }

}
