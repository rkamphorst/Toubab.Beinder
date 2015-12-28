using System;

namespace Beinder
{

    public class PropertyValueChangedEventArgs : EventArgs
    {
        public PropertyValueChangedEventArgs(IProperty property, object newValue)
        {
            Property = property;
            NewValue = newValue;
        }

        public IProperty Property { get; private set; }

        public object NewValue { get; private set; }
    }

}
