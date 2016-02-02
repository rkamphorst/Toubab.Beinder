namespace Toubab.Beinder.Mixins
{
    using System.Windows.Input;
    using Tools;
    using Extend;

    /// <summary>
    /// Mixin to facilitate binding of <see cref="ICommand" /> instances to
    /// <see cref="ICommandSource"/> instances.
    /// </summary>
    public class CommandMixin : CustomMixin<ICommand>
    {
        /// <summary>
        /// Method to be bound to the <see cref="ICommandSource.CanExecuteQuery"/> event.
        /// </summary>
        /// <remarks>
        /// See the <see cref="ICommandSource.CanExecuteQuery"/> documentation for more details.
        /// </remarks>
        public void CanExecuteQuery(CommandCanExecuteArgs args)
        {
            args.Respond(GetObject().CanExecute(args.Parameter));
        }

        /// <summary>
        /// Method to be bound to the <see cref="ICommandSource.Execute"/> event.
        /// </summary>
        /// <remarks>
        /// See the <see cref="ICommandSource.Execute"/> documentation for more details.
        /// </remarks>
        public void Execute(object parameter)
        {
            var o = GetObject();
            if (o.CanExecute(parameter))
            {
                o.Execute(parameter);
            }
        }

        /// <inheritdoc/> 
        public override IMixin CloneWithoutObject()
        {
            return new CommandMixin();
        }
    }


}

