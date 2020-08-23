using EpicMacro.ViewModels;
using System;
using System.Windows.Input;

namespace EpicMacro.Commands.Event_window_commands
{
    public class GetCursorPosCommand : ICommand
    {

        private EventsViewModel ViewModel;
        public GetCursorPosCommand(EventsViewModel vm)
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
            ViewModel.GetCursorPos();
        }
    }
}
