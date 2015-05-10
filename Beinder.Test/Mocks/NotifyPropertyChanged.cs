using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Beinder.Mocks
{

    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetProperty<TProp>(ref TProp field, TProp value, [CallerMemberName] string property = null)
        {
            field = value;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

    }
    
}
