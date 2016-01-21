using System;
using System.Windows.Input;
using Toubab.Beinder.Mixin;
using Toubab.Beinder.Annex;
using Toubab.Beinder.Tools;

namespace Toubab.Beinder.Mixin
{
    /// <summary>
    /// Mixin to facilitate binding of <see cref="ICommand" /> instances to
    /// <see cref="ICommandSource"/> instances.
    /// </summary>
    public class CommandMixin : Mixin<ICommand>
    {
        /// <summary>
        /// Method to be bound to the <see cref="ICommandSource.CanExecuteQuery"/> event.
        /// </summary>
        /// <remarks>
        /// See the <see cref="ICommandSource.CanExecuteQuery"/> documentation for more details.
        /// </remarks>
        public void CanExecuteQuery(CommandCanExecuteArgs args)
        {
            if (!args.CanExecute.HasValue || !args.CanExecute.Value)
                args.CanExecute =
                    GetObject().CanExecute(args.Parameter);
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
        public override IAnnex CloneWithoutObject()
        {
            return new CommandMixin();
        }
    }


}

