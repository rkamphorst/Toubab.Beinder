namespace Toubab.Beinder.Tools
{
    using System;

    /// <summary>
    /// Interface for objects to bind <see cref="System.Windows.Input.ICommand"/> instances to.
    /// </summary>
    /// <remarks>
    /// All events on <see cref="System.Windows.Input.ICommand"/> have method counterparts here,
    /// and vice versa. Except for <see cref="CanExecuteQuery"/>, which has a counterpart 
    /// <see cref="Mixin.CommandMixin.CanExecuteQuery"/>, which is a method.
    /// </remarks>
    public interface ICommandSource
    {
        /// <summary>
        /// Event used to execute a query.
        /// </summary>
        /// <remarks>
        /// This event will be bound to the <see cref="Mixin.CommandMixin.CanExecuteQuery"/>
        /// method. That method will update the <see cref="CommandCanExecuteArgs"/>, which can then 
        /// read by the code that raised the event.
        /// 
        /// Whenever this event occurs, <see cref="Mixin.CommandMixin.CanExecuteQuery"/> will
        /// be called (if bound).
        /// </remarks>
        event Action<CommandCanExecuteArgs> CanExecuteQuery;

        /// <summary>
        /// Event used to execute the command
        /// </summary>
        /// <remarks>
        /// This event will be bound to the <see cref="Mixin.CommandMixin.Execute"/> method
        /// (note: that method effectively hides the <see cref="System.Windows.Input.ICommand.Execute"/> method).
        /// 
        /// Whenever this event is raised, the <see cref="Mixin.CommandMixin.Execute"/> method will be called.
        /// </remarks>
        event Action<object> Execute;

        /// <summary>
        /// Called when "can execute" changes for any parameter
        /// </summary>
        /// <remarks>
        /// This method will be bound to the <see cref="System.Windows.Input.ICommand.CanExecuteChanged"/>
        /// event. Whenever that event occurs, this method will be called.
        /// </remarks>
        void CanExecuteChanged(object source, EventArgs args);
    }
    
}
