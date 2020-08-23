using EpicMacro.Models;
using System;
using System.Windows.Input;

namespace EpicMacro.Commands
{
    public class MoveEventDownCommand : ICommand
    {

        UserEvent UserEvent;
        public MoveEventDownCommand(UserEvent ue)
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
            return UserEvent.ID < MainController.EventsViewModel.UserEventList.Count;
        }

        void ICommand.Execute(object parameter)
        {
            UserEvent.MoveUserEventDown(UserEvent);
        }
    }
}