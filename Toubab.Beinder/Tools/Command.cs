using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Toubab.Beinder.Tools
{
    /// <summary>
    /// Easy-to-use implementation of ICommand
    /// </summary>
    /// <remarks>
    /// Use one of the constructors to provide call-backs for <see cref="Execute()"/>
    /// and <see cref="CanExecute()"/>, and optionally one or more *notify* objects.
    /// 
    /// If you do not supply a callback for <see cref="CanExecute()"/>, <c>true</c> is
    /// assumed as a default return value for any call of <see cref="CanExecute()"/>. 
    /// 
    /// This Command makes sure that it is only ever executing once at a time.
    /// <see cref="Execute"/> is protected by a mutex; and during a call, <see cref="CanExecute()"/>
    /// will return false, regardless of whether the supplied callback returns true
    /// or not.
    /// 
    /// The *notify* objects are used to raise the <see cref="CanExecuteChanged"/> event,
    /// using either the <see cref="INotifyPropertyChanged"/> event, or the
    /// <see cref="INotifyCollectionChanged"/> event on the notify object(s). If notify
    /// object(s) do not implement one of these interfaces, they have no effect. 
    /// 
    /// If you do not supply notify objects, the Target of the <see cref="CanExecute()"/>
    /// callback is automatically configured to be the notify object.
    /// </remarks>
    public class Command : ICommand, IDisposable
    {
        SemaphoreSlim _executeMutex;
        Func<object, Task> _execute;
        Func<object, bool> _canExecute;
        object[] _notify;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// The <paramref name="execute"/> callback will run in a separate thread and
        /// <c>await</c>ed, during which it cannot be called again. <see cref="CanExecute()"/> 
        /// will return <c>false</c> during this time.
        /// </remarks>
        /// <param name="execute">Execute callback</param>
        /// <param name="canExecute">CanExecute callback</param>
        /// <param name="notify">Notify object(s)</param>
        public Command(Action<object> execute, Func<object,bool> canExecute = null, params object[] notify)
            : this(o => { execute(o); return Task.FromResult(0); }, canExecute, notify)
        {
        }
            
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// The <paramref name="execute"/> callback will be <c>await</c>ed, during which
        /// it cannot be called again. <see cref="CanExecute()"/> will return <c>false</c>
        /// during this time. When <paramref name="execute"> is done, <see cref="CanExecute()"/>
        /// will return to calling <paramref name="canExecute"/>.
        /// </remarks>
        /// <param name="execute"><see cref="Execute()"/> callback</param>
        /// <param name="canExecute"><see cref="CanExecute()"/> callback</param>
        /// <param name="notify">Notif object(s)</param>
        public Command(Func<object,Task> execute, Func<object,bool> canExecute = null, params object[] notify)
        {
            _execute = execute;
            _canExecute = canExecute;
            _notify = notify.Length > 0 
                ? notify 
                : canExecute != null && canExecute.Target != null
                    ? new[] { canExecute.Target }
                    : new object[0];

            foreach (var notifier in _notify)
            {
                var cc = notifier as INotifyCollectionChanged;
                if (cc != null)
                    cc.CollectionChanged += NotifierCollectionChanged;
                var pc = notifier as INotifyPropertyChanged;
                if (pc != null)
                    pc.PropertyChanged += NotifierPropertyChanged;
            }

            _executeMutex = new SemaphoreSlim(1, 1);
        }

        #region ICommand members

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Calculates whether this command can execute for given <paramref name="parameter"/>.
        /// </summary>
        /// <remarks>
        /// Also, if <see cref="Execute()"/> is in the process of executing,
        /// this method will return <c>false</c>.
        /// </remarks>
        public bool CanExecute(object parameter)
        {
            return _executeMutex.CurrentCount > 0 && (_canExecute == null || _canExecute(parameter));
        }

        /// <summary>
        /// Execute command with specified parameter.
        /// </summary>
        /// <remarks>
        /// Executes the command asynchronously, and does not allow this command to
        /// execute again until the execution has ended.
        /// 
        /// If you nevertheless call this method while another execution is in progress,
        /// the execution will take place *after* the current execution.
        /// 
        /// While there is an execution in process, <see cref="CanExecute()"/>
        /// will return <c>false</c>.
        /// </remarks>
        public async void Execute(object parameter)
        {
            await _executeMutex.WaitAsync();
            try
            {
                OnCanExecuteChanged();
                await _execute(parameter);
            }
            finally
            {
                _executeMutex.Release();
            }
            OnCanExecuteChanged();
        }

        #endregion

        #region IDisposable members

        public void Dispose()
        {
            if (_notify != null)
            {
                _executeMutex.Dispose();
                foreach (var notifier in _notify)
                {
                    var pc = notifier as INotifyPropertyChanged;
                    var cc = notifier as INotifyCollectionChanged;
                    if (cc != null) cc.CollectionChanged -= NotifierCollectionChanged;
                    if (pc != null) pc.PropertyChanged -= NotifierPropertyChanged;
                }
                _notify = null;
                _canExecute = null;
                _execute = null;
                _executeMutex = null;
            }
        }

        #endregion

        void NotifierPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnCanExecuteChanged();
        }

        void NotifierCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCanExecuteChanged();
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            var evt = CanExecuteChanged;
            if (evt != null)
                evt(this, EventArgs.Empty);
        }


    }
}

