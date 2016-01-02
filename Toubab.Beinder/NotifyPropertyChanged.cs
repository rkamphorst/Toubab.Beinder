using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Toubab.Beinder
{

    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<TProp>(ref TProp field, TProp value, [CallerMemberName] string property = null)
        {
            if (!Equals(field, value))
            {
                field = value;
                OnPropertyChanged(property);
                return true;
            }
            return false;
        }

        protected void OnPropertyChanged([CallerMemberName] string property = null)
        {
            var evt = PropertyChanged;
            if (evt != null)
                evt(this, new PropertyChangedEventArgs(property));
        }

    }
    
}
