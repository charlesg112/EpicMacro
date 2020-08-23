using EpicMacro.Commands;
using EpicMacro.Models;
using EpicMacro.Res;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.RightsManagement;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;

namespace EpicMacro.ViewModels
{
    public class EventsViewModel : INotifyPropertyChanged
    {

        #region Attributes
        private UserEvent _SelectedUserEvent;
        private bool _LoopModeEnabled, _InfinityModeEnabled;
        private int _LoopCount;
        public ObservableCollection<UserEvent> UserEventList = new ObservableCollection<UserEvent>();
        public ObservableCollection<SelectableUserEvent> UserEventComboBoxItems { get; set; }
        public UserEvent SelectedUserEvent { get { return _SelectedUserEvent; } set { _SelectedUserEvent = value; OnPropertyChanged("SelectedUserEvent"); } }

        public bool LoopModeEnabled { get { return _LoopModeEnabled; } set { _LoopModeEnabled = value; OnPropertyChanged("LoopModeEnabled"); PopUpManager.UpdateLoopCountVisibility(_LoopModeEnabled); } }
        public bool InfinityModeEnabled { get { return _InfinityModeEnabled; } set { _InfinityModeEnabled = value; OnPropertyChanged("InfinityModeEnabled"); PopUpManager.UpdateLoopCountEnabled(_InfinityModeEnabled); } }
        public int LoopCount { get { return _LoopCount; } set { _LoopCount = value; OnPropertyChanged("LoopCount"); } }
        public PopUpManager PopUpManager { get; private set; }
        public bool IsExecuting { get; private set; }
        public Thread CurrentExecutionThread { get; set; }
        public Thread ListenerThread { get; set; }
        public ICommand ExecuteCommand { get; private set; }
        public ICommand AddEventCommand { get; private set; }
        public ICommand AbortCommand { get; private set; }

        #endregion

        public EventsViewModel()
        {
            UserEventComboBoxItems = new ObservableCollection<SelectableUserEvent>();
            ExecuteCommand = new ExecuteCommand(this);
            AddEventCommand = new AddEventCommand(this);
            AbortCommand = new AbortCommand(this);
            PopUpManager = new PopUpManager(LoopModeEnabled);
            ListenerThread = new Thread(new ThreadStart(Listen));
            ListenerThread.SetApartmentState(ApartmentState.STA);
            ListenerThread.Start();
            IsExecuting = false;
            AddComboBoxValues();
        }

        internal void Execute()
        {
            CurrentExecutionThread = new Thread(new ThreadStart(ExecuteImpl));
            CurrentExecutionThread.Start();
            //Task.Run(() => ExecuteImpl());
        }

        internal void AbortExecution()
        {
            if (CurrentExecutionThread != null)
            {
                CurrentExecutionThread.Abort();
                IsExecuting = false;
            }
        }

        internal void ExecuteImpl()
        {
            IsExecuting = true;
            if (ValidateExecution())
            {
                try
                {
                    UserEvent.ParseLoopContents();

                    if (LoopModeEnabled && !InfinityModeEnabled)
                    {

                        for (int i = 0; i < LoopCount - 1; i++)
                        {
                            try
                            {
                                foreach (UserEvent userEvent in UserEventList)
                                {
                                    if (userEvent.IdentationLevel == 0) userEvent.Perform();
                                }
                            }
                            catch
                            {
                                Debug.WriteLine("Error occured");
                            }
                        }
                    }

                    if (LoopModeEnabled && InfinityModeEnabled)
                    {

                        while (true)
                        {
                            try
                            {
                                foreach (UserEvent userEvent in UserEventList)
                                {
                                    if (userEvent.IdentationLevel == 0) userEvent.Perform();
                                }
                            }
                            catch
                            {
                                Debug.WriteLine("Error occured");
                            }
                        }
                    }

                    else
                    {
                        try
                        {
                            foreach (UserEvent userEvent in UserEventList)
                            {
                                if (userEvent.IdentationLevel == 0) userEvent.Perform();
                            }
                        }
                        catch
                        {
                            Debug.WriteLine("Error occured");
                        }
                    }
                }

                catch
                {
                    Debug.WriteLine("An error occured");
                    //CurrentExecutionThread.Abort();
                }
                

            }

            else 
            {
                Debug.WriteLine("Execution could not be performed. Check the Execution Options");
            }

            IsExecuting = false;
        }

        internal void AddUserEvent()
        {
            UserEventList.Add(new UserEvent(UserEventType.Click));
        }

        internal bool ValidateExecution()
        {
            bool output = true;
            if (LoopModeEnabled && !InfinityModeEnabled && LoopCount <= 0) output = false;
            return output;
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        public ObservableCollection<SelectableUserEvent> GetComboBoxValues()
        {

            ObservableCollection<SelectableUserEvent> output = new ObservableCollection<SelectableUserEvent>();

            foreach (UserEventType ue in Enum.GetValues(typeof(UserEventType)))
            {
                output.Add(new SelectableUserEvent(ue));
            }

            return output;

        }

        public void AddComboBoxValues()
        {
            foreach (UserEventType ue in Enum.GetValues(typeof(UserEventType)))
            {
                if (ue != UserEventType.Unknown) UserEventComboBoxItems.Add(new SelectableUserEvent(ue));
            }
        }

        public void Listen()
        {
            while (true)
            {
                if (Keyboard.IsKeyDown(Key.P))
                {
                    Debug.WriteLine("P Key pressed");
                    AbortExecution();
                    Thread.Sleep(100);
                }

                Thread.Sleep(10);
            }
        }


        public void RemoveUserEvent(UserEvent userEvent)
        {
            UserEventList.Remove(userEvent);
            RefreshIndexes();
        }

        public void RefreshIndexes()
        {
            for (int i = 0; i < UserEventList.Count; i++)
            {
                UserEventList[i].ID = i + 1;
            }

            UserEvent.StaticID = UserEventList.Count;
        }

    }
}
