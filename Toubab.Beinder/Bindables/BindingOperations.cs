namespace Toubab.Beinder.Bindables
{
    using System;

    [Flags]
    public enum BindingOperations
    {
        None = 0,
        Read = 1,
        Broadcast = 2,
        HandleBroadcast = 4,
        All = Read | Broadcast | HandleBroadcast
    }
}
