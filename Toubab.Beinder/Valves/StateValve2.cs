namespace Toubab.Beinder.Valves
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Tools;
    using Bindables;

    public class StateValve2 : Valve2
    {
        // the secret object array.
        // it is guaranteed that no object array outside this class
        // can hold the same array as this one; and that is what we want.
        static readonly object[] _secret = { new object() };

        object[] _values = _secret;

        public StateValve2(Fixture fixture)
            : base(fixture)
        {
        }

        #region Activate

        public async Task<bool> Activate(int tagToActivate)
        {
            var conduit = Fixture.Conduits.FirstOrDefault(c => Equals(c.Tag, tagToActivate));
            return await Activate(conduit);
        }

        public async Task<bool> Activate(Conduit toActivate)
        {
            if (toActivate == null)
                return false;
            var prop = toActivate.Bindable as IProperty;
            if (prop == null)
                return false;

            object[] payload;
            using (toActivate.Attach())
            {
                payload = new[] { prop.Value };
            }

            await BroadcastAsync(toActivate, payload);
            return true;
        }

        #endregion

        #region Broadcast

        protected override async Task<bool> BroadcastAsync(Conduit sender, object[] payload)
        {
            // important! any event that emanates from an IProperty source is
            // interpreted as a property change. Therefore, the payload is 
            // replaced with the new value of the property; the original event 
            // arguments are discarded.
            var prop = sender.Bindable as IProperty;
            if (prop != null)
            {
                using (var attachment = sender.Attach())
                {
                    payload = new[] { prop.Value };
                }
            }

            lock (_secret)
            {
                if (!Equals(_values, payload) && !_values.SequenceEqual(payload))
                {
                    _values = payload;
                }
                else
                {
                    return false;
                }
            }

            var result = await base.BroadcastAsync(sender, payload);
            OnUpdated();
            return result;
        }

        #endregion

        #region Updated event

        public event EventHandler Updated;

        void OnUpdated()
        {
            var evt = Updated;
            if (evt != null)
            {
                evt(this, EventArgs.Empty);
            }
        }

        #endregion
    }

}
