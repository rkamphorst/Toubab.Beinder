namespace Toubab.Beinder.Tools
{
    /// <summary>
    /// Arguments used to capture the result of <see cref="System.Windows.Input.ICommand.CanExecute()"/>.
    /// </summary>
    /// <remarks>
    /// See also <see cref="Mixin.CommandMixin.CanExecuteQuery"/>,
    /// <see cref="ICommandSource.CanExecuteQuery"/> and <see cref="CommandSource.CanExecuteQuery"/>.
    /// </remarks>
    public class CommandCanExecuteArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameter">The parameter to run the query for.</param>
        public CommandCanExecuteArgs(object parameter) 
        {
            Parameter = parameter;
        }

        /// <summary>
        /// The parameter to run the query for
        /// </summary>
        public object Parameter { get; private set; }

        /// <summary>
        /// Stores the result of the query
        /// </summary>
        /// <remarks>
        /// Initially, the value is <c>(Nullable<bool>)null</c>. Event handlers
        /// set this value to either true or false.
        /// </remarks>
        public bool? CanExecute { get; set; }

    }
    
}
