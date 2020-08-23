using EpicMacro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EpicMacro.Commands
{
    public class MoveEventUpCommand : ICommand
    {

        UserEvent UserEvent;
        public MoveEventUpCommand(UserEvent ue)
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
            return UserEvent.ID > 1;
        }

        void ICommand.Execute(object parameter)
        {
            UserEvent.MoveUserEventUp(UserEvent);
        }
    }
}