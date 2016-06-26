namespace Toubab.Beinder.Tools
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Easy-to-use implementation of ICommandSource
    /// </summary>
    public class CommandSource : ICommandSource
    {
        readonly Action<Func<object,bool>> _canExecuteChanged;

        /// <summary>
        /// Constructor to be used by subclasses
        /// </summary>
        protected CommandSource() 
        {
            _canExecuteChanged = null;
        }

        /// <summary>
        /// Initializes a <see cref="CommandSource"/> with a <c>null</c> parameter
        /// and "can execute changed" handler <paramref name="canExecuteChanged"/>.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="CommandSource(object, Action{bool})"/> with the first parameter
        /// set to <c>null</c>.
        /// </remarks>
        public CommandSource(Action<bool> canExecuteChanged) 
            : this(null, canExecuteChanged)
        {
        }

        /// <summary>
        /// Initializes a <see cref="CommandSource"/> with a single parameter.
        /// </summary>
        /// <param name="parameter">Parameter for the command</param>
        /// <param name="canExecuteChanged">What to do when <see cref="CanExecuteChanged"/> is called.
        /// The boolean parameter of this callback will indicate whether execution is enabled
        /// for given <paramref name="parameter"/>.
        /// </param>
        public CommandSource(object parameter, Action<bool> canExecuteChanged)
            : this(f => canExecuteChanged(f(parameter)))
        {
        }

        /// <summary>
        /// Initializes a <see cref="CommandSource"/> instance.
        /// </summary>
        /// <param name="canExecuteChanged">Execute changed callback.
        /// The first parameter to this callback is a <see cref="Func{A,B}"/>
        /// with which the <paramref name="canExecuteChanged"/> callback can determine
        /// for a parameter whether execution is enabled or not.
        /// </param>
        public CommandSource(Action<Func<object,bool>> canExecuteChanged)
        {
            _canExecuteChanged = canExecuteChanged;
        }

        /// <summary>
        /// Event used to execute a query by <see cref="CanExecute"/>
        /// </summary>
        /// <remarks>
        /// This event will be bound to the <see cref="Mixins.CommandMixin.CanExecuteQuery"/>
        /// method. That method will update the <see cref="CommandCanExecuteArgs"/>, which is then 
        /// read by <see cref="CanExecute"/> to determine whether any bound 
        /// <see cref="System.Windows.Input.ICommand"/> can execute.
        /// 
        /// Whenever this event occurs, <see cref="Mixins.CommandMixin.CanExecuteQuery"/> will
        /// be called (if bound).
        /// 
        /// </remarks>
        public event Action<CommandCanExecuteArgs> CanExecuteQuery;
            
        /// <summary>
        /// Event used to execute the command
        /// </summary>
        /// <remarks>
        /// This event will be bound to the <see cref="Mixins.CommandMixin.Execute"/> method
        /// (note: that method effectively hides the <see cref="System.Windows.Input.ICommand.Execute"/> method).
        /// Whenever this event is raised, that method will be called.
        /// </remarks>
        public event Action<object> Execute;

        /// <summary>
        /// Called when "can execute" changes for any parameter
        /// </summary>
        /// <remarks>
        /// This method will be bound to the <see cref="System.Windows.Input.ICommand.CanExecuteChanged"/>
        /// event. Whenever that event occurs, this method will be called.
        /// </remarks>
        public virtual void CanExecuteChanged(object source, EventArgs args)
        {
            if (_canExecuteChanged != null)
                _canExecuteChanged(CanExecute);
        }

        /// <summary>
        /// Uses <see cref="CanExecuteQuery"/> to find out if the command can execute
        /// </summary>
        /// <remarks>
        /// See the <see cref="CanExecuteQuery"/> documentation for more details on the
        /// used mechanism.
        /// 
        /// Note: <see cref="CanExecute"/> returns <c>true</c> if *any* of the handlers
        /// of <see cref="CanExecuteQuery"/> indicates that the command can be executed.
        /// </remarks>
        public virtual bool CanExecute(object parameter)
        {
            var evt = CanExecuteQuery;
            bool result = false;
            if (evt != null)
            {
                var args = new CommandCanExecuteArgs(parameter);
                evt(args);
                if (args.CanExecute.HasValue)
                    result = args.CanExecute.Value;
            }
            return result;
        }

        /// <summary>
        /// Raises the execute event.
        /// </summary>
        /// <remarks>
        /// Does *not* check whether the command can actually execute!
        /// </remarks>
        public virtual bool OnExecute(object parameter)
        {
            var evt = Execute;
            if (evt != null)
                evt(parameter);
            return true;
        }

    }
}
