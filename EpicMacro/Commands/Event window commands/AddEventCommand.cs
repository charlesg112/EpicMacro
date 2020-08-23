using EpicMacro.ViewModels;
using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace EpicMacro.Commands
{
    public class AddEventCommand : ICommand
    {

        private EventsViewModel ViewModel;
        public AddEventCommand(EventsViewModel vm)
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
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            ViewModel.AddUserEvent();
        }
    }
}
