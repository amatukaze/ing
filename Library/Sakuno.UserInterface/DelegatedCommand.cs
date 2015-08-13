using System;
using System.Windows.Input;

namespace Sakuno
{
    public class DelegatedCommand : ICommand
    {
        Action r_Command;
        Func<bool> r_CanExecute;

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegatedCommand(Action rpCommand) : this(rpCommand, null) { }
        public DelegatedCommand(Action rpCommand, Func<bool> rpCanExecute)
        {
            if (rpCommand == null)
                throw new ArgumentNullException("rpCommand");

            r_Command = rpCommand;
            r_CanExecute = rpCanExecute;
        }

        void ICommand.Execute(object rpParameter)
        {
            r_Command();
        }
        bool ICommand.CanExecute(object rpParameter)
        {
            return r_CanExecute == null || r_CanExecute();
        }
    }
    public class DelegatedCommand<T> : ICommand
    {
        Action<T> r_Command;
        Func<T, bool> r_CanExecute;

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegatedCommand(Action<T> rpCommand) : this(rpCommand, null) { }
        public DelegatedCommand(Action<T> rpCommand, Func<T, bool> rpCanExecute)
        {
            if (rpCommand == null)
                throw new ArgumentNullException();

            r_Command = rpCommand;
            r_CanExecute = rpCanExecute;
        }

        void ICommand.Execute(object rpParameter)
        {
            r_Command((T)rpParameter);
        }
        bool ICommand.CanExecute(object rpParameter)
        {
            return r_CanExecute == null || r_CanExecute((T)rpParameter);
        }
    }
}
