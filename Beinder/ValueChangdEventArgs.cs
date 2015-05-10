using System;
using System.Linq;
using System.Collections.Generic;

namespace Beinder
{

    public class ValueChangedEventArgs : EventArgs
    {
        public ValueChangedEventArgs(object newValue)
        {
            NewValue = newValue;
        }

        public object NewValue { get; private set; }
    }

}
