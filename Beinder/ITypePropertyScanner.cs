using System.Collections.Generic;
using System;

namespace Beinder
{

    public interface ITypePropertyScanner
    {
        IEnumerable<IProperty> Scan(Type type);
    }

}
