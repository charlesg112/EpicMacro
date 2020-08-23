using EpicMacro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EpicMacro.Commands
{
    public class ExecuteCommand : ICommand
    {

        private EventsViewModel ViewModel;
        public ExecuteCommand(EventsViewModel vm)
        {
            ViewModel = vm;
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
            return (ViewModel.UserEventList.Count != 0);
        }

        void ICommand.Execute(object parameter)
        {
            ViewModel.Execute();
        }
    }
}
