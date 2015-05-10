using System;
using System.Linq;

namespace Beinder
{

    public interface IPropertyPathParser 
    {
        PropertyPath Parse(string name);
    }

}
