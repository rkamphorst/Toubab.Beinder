namespace Toubab.Beinder.Bindables
{
    using System.Text;

    public static class BindableUtilities
    {

        public static bool CanRead(this IBindable self)
        {
            return (self.Capabilities & BindingOperations.Read) != 0;
        }

        public static bool CanHandleBroadcast(this IBindable self)
        {
            return (self.Capabilities & BindingOperations.HandleBroadcast) != 0;
        }

        public static bool CanBroadcast(this IBindable self)
        {
            return (self.Capabilities & BindingOperations.Broadcast) != 0;
        }

        public static bool CanBroadcastOrRead(this IBindable self)
        {
            return (self.Capabilities & (BindingOperations.Broadcast | BindingOperations.Read)) != 0;
        }

        const char CAN_HANDLE_SYMBOL = '♀';
        const char CAN_BROADCAST_SYMBOL = '♂';
        const char CAN_BROADCAST_AND_HANDLE_SYMBOL = '⚥';
        const char CAN_READ_SYMBOL = 'P'; // "can read" is synonym to "is property", hence "P".

        public static string GetCapabilitiesString(this IBindable self)
        {
            var sb = new StringBuilder();
            var caps = self.Capabilities;
            if ((caps & BindingOperations.Read) != 0)
                sb.Append(CAN_READ_SYMBOL);

            var bAndHCaps = self.Capabilities & (BindingOperations.Broadcast | BindingOperations.HandleBroadcast);
            switch (bAndHCaps)
            {
                case BindingOperations.Broadcast | BindingOperations.HandleBroadcast:
                    sb.Append(CAN_BROADCAST_AND_HANDLE_SYMBOL);
                    break;
                case BindingOperations.Broadcast:
                    sb.Append(CAN_BROADCAST_SYMBOL);
                    break;
                case BindingOperations.HandleBroadcast:
                    sb.Append(CAN_HANDLE_SYMBOL);
                    break;
            }

            return sb.ToString();
        }
    }
}
