using EpicMacro.Commands;
using EpicMacro.Commands.Event_window_commands;
using EpicMacro.Models;
using EpicMacro.Res;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace EpicMacro.ViewModels
{
    public class EventsViewModel : INotifyPropertyChanged
    {

        #region Attributes

            #region Unsorted Attributes
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
        #endregion

            #region Commands
        public ICommand ExecuteCommand { get; private set; }
        public ICommand AddEventCommand { get; private set; }
        public ICommand AbortCommand { get; private set; }
        public ICommand GetCursorPosCommand { get; private set; }

        #endregion

            #region Cursor Info

        private int _CursorInfoX, _CursorInfoY;
        public int CursorInfoX { get { return _CursorInfoX; } set { _CursorInfoX = value; OnPropertyChanged("CursorInfoX"); } }
        public int CursorInfoY { get { return _CursorInfoY; } set { _CursorInfoY = value; OnPropertyChanged("CursorInfoY"); } }

        #endregion

        #endregion

        #region Constructor
        public EventsViewModel()
        {
            UserEventComboBoxItems = new ObservableCollection<SelectableUserEvent>();
            
            // Calling the constructor of all commands
            ExecuteCommand = new ExecuteCommand(this);
            AddEventCommand = new AddEventCommand(this);
            AbortCommand = new AbortCommand(this);
            GetCursorPosCommand = new GetCursorPosCommand(this);

            // Other stuff
            PopUpManager = new PopUpManager(LoopModeEnabled);
            IsExecuting = false;
            AddComboBoxValues();

            // ListenerThread creation and start
            ListenerThread = new Thread(new ThreadStart(Listen));
            ListenerThread.SetApartmentState(ApartmentState.STA);
            ListenerThread.Start();

        }

        #endregion

        #region Command Implementation

            #region Execution Methods
        internal void Execute()
        {
            CurrentExecutionThread = new Thread(new ThreadStart(ExecuteImpl));
            CurrentExecutionThread.Start();
        }

        internal void AbortExecution()
        {
            if (CurrentExecutionThread != null)
            {
                CurrentExecutionThread.Abort();
                IsExecuting = false;
            }
        }

        internal bool ValidateExecution()
        {
            bool output = true;
            if (LoopModeEnabled && !InfinityModeEnabled && LoopCount <= 0) output = false;
            return output;
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

        #endregion

            #region Information Methods

        internal void GetCursorPos()
        {
            MouseOperations.MousePoint xy = MouseOperations.GetCursorPosition();
            CursorInfoX = xy.X;
            CursorInfoY = xy.Y;
        }

        #endregion

            #region UserEventList Methods
        internal void AddUserEvent()
        {
            UserEventList.Add(new UserEvent(UserEventType.Click));
        }

        public void RemoveUserEvent(UserEvent userEvent)
        {
            UserEventList.Remove(userEvent);
            UserEvent.RefreshIndexes();
        }

        #endregion

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Public and Thread Methods

            #region UserEvent Combobox generation
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

        #endregion

            #region Thread Methods
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

                if (Keyboard.IsKeyDown(Key.O))
                {
                    GetCursorPos();
                    Thread.Sleep(10);
                }

                if (Keyboard.IsKeyDown(Key.Left))
                {
                    Application.Current.Dispatcher.Invoke(() => UserEvent.MoveSelectedUserEventUp());
                    Thread.Sleep(100);
                }

                if (Keyboard.IsKeyDown(Key.Right))
                {
                    Application.Current.Dispatcher.Invoke(() => UserEvent.MoveSelectedUserEventDown());
                    Thread.Sleep(100);
                }

                if (Keyboard.IsKeyDown(Key.Delete))
                {
                    Application.Current.Dispatcher.Invoke(() => UserEvent.DeleteSelectedUserEventDown());
                    Thread.Sleep(100);
                }

                Thread.Sleep(10);
            }
        }

        #endregion

        #endregion

    }
}
