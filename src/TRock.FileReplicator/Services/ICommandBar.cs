using System;
using System.Windows.Input;

namespace TRock.FileReplicator.Services
{
    public interface ICommandBar
    {
        #region Methods

        ICommandBar AddCommand(string displayName, ICommand command);

        ICommandBar AddCommand(string displayName, ICommand command, object commandParameter);

        ICommandBar AddCommand(CommandModel commandModel);

        ICommandBar AddSeparator();

        ICommandBar AddSeparator(string displayName);

        ICommandBar AddSubmenu(string displayName);

        ICommandBar Clear();

        ICommandBar Remove(Func<CommandModel, bool> predicate);

        #endregion Methods
    }
}