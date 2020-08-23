using EpicMacro.ViewModels;
using System;
using System.Windows.Input;

namespace EpicMacro.Commands
{
    class AbortCommand : ICommand
    {

        private EventsViewModel EventsViewModel;

        public AbortCommand(EventsViewModel vm)
        {
            EventsViewModel = vm;
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
            return EventsViewModel.IsExecuting;
        }

        void ICommand.Execute(object parameter)
        {
            EventsViewModel.AbortExecution();
        }
    }
}
