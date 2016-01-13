using System;

namespace Toubab.Beinder.Valve
{

    public class BindableBroadcastEventArgs : EventArgs
    {
        public BindableBroadcastEventArgs(IBindable property, object[] argument)
        {
            Source = property;
            Argument = argument;
        }

        public IBindable Source { get; private set; }

        public object[] Argument { get; private set; }
    }

}
