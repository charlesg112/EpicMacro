using EpicMacro.Models;
using System;
using System.Windows.Input;

namespace EpicMacro.Commands
{
    public class RemoveEventCommand : ICommand
    {

        UserEvent UserEvent;
        public RemoveEventCommand(UserEvent ue)
        {
            UserEvent = ue;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            MainController.EventsViewModel.RemoveUserEvent(UserEvent);
        }
    }
}
