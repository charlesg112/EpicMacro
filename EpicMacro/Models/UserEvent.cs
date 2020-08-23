using EpicMacro.Commands;
using EpicMacro.Exceptions;
using EpicMacro.Res;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;

namespace EpicMacro.Models
{
    public enum UserEventType
    {
        Click,
        KeyPress,
        Delay,
        LongClick,
        Loop,
        EndLoop,
        Unknown
    }

    public class UserEvent : INotifyPropertyChanged
    {

        #region Attributes

            #region Looping Attributes

        private int _IdentationLevel;
        private List<UserEvent> _LoopContents = new List<UserEvent>();
        private int _LoopEndIndex;
        public int IdentationLevel { get { return _IdentationLevel; } set { _IdentationLevel = value; OnPropertyChanged("IdentationLevel"); } }
        public List<UserEvent> LoopContents { get { return _LoopContents; } set { _LoopContents = value; OnPropertyChanged("LoopContents"); } }
        public int LoopEndIndex { get { return _LoopEndIndex; } set { _LoopEndIndex = value; OnPropertyChanged("LoopEndIndex"); } }

        #endregion

            #region Value Fields

        // Private
        private string _KeyPressValueField;
        private float _DelayValueField;
        private int _LongClickDurationValueField;
        private int _ClickXValueField, _ClickYValueField, _LongClickXValueField, _LongClickYValueField;
        private int _IterationsValueField;

        // Public
        public string KeyPressValueField { get { return _KeyPressValueField; } set { _KeyPressValueField = value; OnPropertyChanged("KeyPressValueField"); } }
        public float DelayValueField { get { return _DelayValueField; } set { _DelayValueField = value; OnPropertyChanged("DelayValueField"); } }
        public int LongClickDurationValueField { get { return _LongClickDurationValueField; } set { _LongClickDurationValueField = value; OnPropertyChanged("LongClickDurationValueField"); } }
        public int ClickXValueField { get { return _ClickXValueField; } set { _ClickXValueField = value; OnPropertyChanged("ClickXValueField"); } }
        public int ClickYValueField { get { return _ClickYValueField; } set { _ClickYValueField = value; OnPropertyChanged("ClickYValueField"); } }
        public int LongClickXValueField { get { return _LongClickXValueField; } set { _LongClickXValueField = value; OnPropertyChanged("LongClickXValueField"); } }
        public int LongClickYValueField { get { return _LongClickYValueField; } set { _LongClickYValueField = value; OnPropertyChanged("LongClickYValueField"); } }
        public int IterationsValueField { get { return _IterationsValueField; } set { _IterationsValueField = value; OnPropertyChanged("IterationsValueField"); } }

        #endregion

            #region Fields visibility

        // Private
        private string _ClickValueFieldVisibility, _KeyPressValueFieldVisibility, _DelayValueFieldVisibility, _LongClickValueFieldVisibility, _IterationsValueFieldVisibility;

        // Public
        public string ClickValueFieldVisibility { get { return _ClickValueFieldVisibility; } set { _ClickValueFieldVisibility = value; OnPropertyChanged("ClickValueFieldVisibility"); } }
        public string KeyPressValueFieldVisibility { get { return _KeyPressValueFieldVisibility; } set { _KeyPressValueFieldVisibility = value; OnPropertyChanged("KeyPressValueFieldVisibility"); } }
        public string DelayValueFieldVisibility { get { return _DelayValueFieldVisibility; } set { _DelayValueFieldVisibility = value; OnPropertyChanged("DelayValueFieldVisibility"); } }
        public string LongClickValueFieldVisibility { get { return _LongClickValueFieldVisibility; } set { _LongClickValueFieldVisibility = value; OnPropertyChanged("LongClickValueFieldVisibility"); } }
        public string IterationsValueFieldVisibility { get { return _IterationsValueFieldVisibility; } set { _IterationsValueFieldVisibility = value; OnPropertyChanged("IterationsValueFieldVisibility"); } }

        #endregion

            #region Unsorted Attributes
        public static int StaticID;

        // Private
        private int _ID;
        private UserEventType _EventType;
        private SelectableUserEvent _SelectableUserEvent;

        // Public
        public UserEventType EventType { get { return _EventType; } set { _EventType = value; OnPropertyChanged("EventType"); } }
        public SelectableUserEvent SelectableUserEvent { get { return _SelectableUserEvent; } set { _SelectableUserEvent = value; OnPropertyChanged("SelectableUserEvent"); } }
        public int ID { get { return _ID; } set { _ID = value; OnPropertyChanged("ID"); } }

        #endregion

            #region Commands

        public ICommand RemoveCommand { get; private set; }
        public ICommand MoveEventUpCommand { get; private set; }
        public ICommand MoveEventDownCommand { get; private set; }

        #endregion

        #endregion

        #region Constructors
        public UserEvent(UserEventType userEventType)
        {
            EventType = userEventType;
            
            // Commands
            RemoveCommand = new RemoveEventCommand(this);
            MoveEventUpCommand = new MoveEventUpCommand(this);
            MoveEventDownCommand = new MoveEventDownCommand(this);

            SelectableUserEvent = MainController.EventsViewModel.UserEventComboBoxItems[UserEventValueConverter.GetComboBoxID(userEventType)];
            StaticID++;
            ID = StaticID;
            SetVisibilities();
        }

        public UserEvent(UserEventType userEventType, int id)
        {
            EventType = userEventType;
            
            // Commands
            RemoveCommand = new RemoveEventCommand(this);
            MoveEventUpCommand = new MoveEventUpCommand(this);
            MoveEventDownCommand = new MoveEventDownCommand(this);

            SelectableUserEvent = new SelectableUserEvent(userEventType);
            ID = id;
            SetVisibilities();
        }

        #endregion

        #region Private and Public Instance Methods

        public void SetVisibilities()
        {
            switch (EventType)
            {
                case UserEventType.Click:
                    KeyPressValueFieldVisibility = "Collapsed"; DelayValueFieldVisibility = "Collapsed"; ClickValueFieldVisibility = "Visible"; LongClickValueFieldVisibility = "Collapsed";
                    IterationsValueFieldVisibility = "Collapsed"; break;
                case UserEventType.KeyPress:
                    KeyPressValueFieldVisibility = "Visible"; DelayValueFieldVisibility = "Collapsed"; ClickValueFieldVisibility = "Collapsed"; LongClickValueFieldVisibility = "Collapsed";
                    IterationsValueFieldVisibility = "Collapsed"; break;
                case UserEventType.Delay:
                    KeyPressValueFieldVisibility = "Collapsed"; DelayValueFieldVisibility = "Visible"; ClickValueFieldVisibility = "Collapsed"; LongClickValueFieldVisibility = "Collapsed";
                    IterationsValueFieldVisibility = "Collapsed"; break;
                case UserEventType.LongClick:
                    KeyPressValueFieldVisibility = "Collapsed"; DelayValueFieldVisibility = "Collapsed"; ClickValueFieldVisibility = "Collapsed"; LongClickValueFieldVisibility = "Visible";
                    IterationsValueFieldVisibility = "Collapsed"; break;
                case UserEventType.Loop:
                    KeyPressValueFieldVisibility = "Collapsed"; DelayValueFieldVisibility = "Collapsed"; ClickValueFieldVisibility = "Collapsed"; LongClickValueFieldVisibility = "Collapsed";
                    IterationsValueFieldVisibility = "Visible"; break;
                case UserEventType.EndLoop:
                    KeyPressValueFieldVisibility = "Collapsed"; DelayValueFieldVisibility = "Collapsed"; ClickValueFieldVisibility = "Collapsed"; LongClickValueFieldVisibility = "Collapsed";
                    IterationsValueFieldVisibility = "Collapsed"; break;
            }
        }

        public void UpdateValues()
        {
            EventType = SelectableUserEvent.UserEventType;
            SetVisibilities();
        }

        #endregion

        #region Perform

        public void Perform()
        {
            Debug.WriteLine($"{EventType} performed");
            switch (EventType)
            {
                case UserEventType.Click:
                    ClickEvent(); break;
                case UserEventType.Delay:
                    DelayEvent(); break;
                case UserEventType.KeyPress:
                    KeyPressEvent(); break;
                case UserEventType.LongClick:
                    LongClickEvent(); break;
                case UserEventType.Loop:
                    LoopEvent(); break;
            }
        }

        #region Events Implementation

        public void ClickEvent()
        {
            MouseOperations.MouseLeftClick(ClickXValueField, ClickYValueField);
        }

        public void DelayEvent()
        {
            Thread.Sleep((int)(DelayValueField * 1000));
        }

        public void KeyPressEvent()
        {

        }

        public void LongClickEvent()
        {
            MouseOperations.MouseLeftClickLong(LongClickXValueField, LongClickYValueField, LongClickDurationValueField);
        }

        public void LoopEvent() 
        {
            for(int i = 0; i < IterationsValueField; i++)
            {
                foreach (UserEvent userEvent in LoopContents)
                {
                    userEvent.Perform();
                }
            }
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

        #region Static Methods

        /// <summary>
        /// Calculates the indentation level of each items in the UserEventList
        /// </summary>
        public static void CalculateIndentation()
        {
            int ind; // Identation level

            for (int i = 0; i < MainController.EventsViewModel.UserEventList.Count; i++)
            {
                ind = 0;
                for (int j = 0; j < i; j++)
                {
                    if (MainController.EventsViewModel.UserEventList[j].EventType == UserEventType.Loop) ind += 1;
                    else if (MainController.EventsViewModel.UserEventList[j].EventType == UserEventType.EndLoop) ind -= 1;
                }

                if (MainController.EventsViewModel.UserEventList[i].EventType == UserEventType.EndLoop) ind -= 1;

                MainController.EventsViewModel.UserEventList[i].IdentationLevel = ind;
            }
        }

        /// <summary>
        /// Parses the contents of each loop based on the identation level.
        /// Adds the contents of each loop to the LoopContents attribute.
        /// </summary>
        public static void ParseLoopContents()
        {
            CalculateIndentation();
            UserEvent currentUserEvent;
            for (int i = 0; i < MainController.EventsViewModel.UserEventList.Count; i++)
            {
                //Debug.WriteLine($"LoopI : Looking at ID {MainController.EventsViewModel.UserEventList[i].ID} with type {MainController.EventsViewModel.UserEventList[i].EventType} and indentation {MainController.EventsViewModel.UserEventList[i].IdentationLevel}");
                currentUserEvent = MainController.EventsViewModel.UserEventList[i];
                currentUserEvent.LoopEndIndex = -1;
                if (currentUserEvent.EventType == UserEventType.Loop)
                {
                    //Debug.WriteLine($"Found the beginning of the loop at ID { currentUserEvent.ID}");

                    for (int j = i + 1; j < MainController.EventsViewModel.UserEventList.Count; j++)
                    {
                        //Debug.WriteLine($"LoopJ : Looking at ID {MainController.EventsViewModel.UserEventList[j].ID} with type {MainController.EventsViewModel.UserEventList[j].EventType} and indentation {MainController.EventsViewModel.UserEventList[j].IdentationLevel}");
                        if (MainController.EventsViewModel.UserEventList[j].IdentationLevel == currentUserEvent.IdentationLevel + 1)    // Current j is part of the loop
                        {
                            currentUserEvent.LoopContents.Add(MainController.EventsViewModel.UserEventList[j]);
                        }

                        else if (MainController.EventsViewModel.UserEventList[j].IdentationLevel == currentUserEvent.IdentationLevel && 
                            MainController.EventsViewModel.UserEventList[j].EventType == UserEventType.EndLoop)                         // Current j is the end of the loop
                        {
                            currentUserEvent.LoopEndIndex = j;
                            Debug.WriteLine($"Found the end of the loop at ID { MainController.EventsViewModel.UserEventList[j].ID}");
                            break;
                        }
                    }

                    if (currentUserEvent.LoopEndIndex == -1)
                    {
                        //Debug.WriteLine($"LoopEndIndex is -1 for event with ID {currentUserEvent.ID}");
                        throw new LoopIdentationException($"Could not parse the ending of the loop with the ID \"{currentUserEvent.ID}\"");
                    }

                }
            }
        }

        /// <summary>
        /// Refreshes the indexes of all UserEvents listed
        /// </summary>
        public static void RefreshIndexes()
        {
            for (int i = 0; i < MainController.EventsViewModel.UserEventList.Count; i++)
            {
                MainController.EventsViewModel.UserEventList[i].ID = i + 1;
            }

            UserEvent.StaticID = MainController.EventsViewModel.UserEventList.Count;
        }

        public static void MoveUserEventUp(UserEvent userEvent)
        {
            if (userEvent.ID >= 2)
            {
                MainController.EventsViewModel.UserEventList.Move(userEvent.ID - 1, userEvent.ID - 2);
            }

            RefreshIndexes();
        }

        public static void MoveUserEventDown(UserEvent userEvent)
        {
            if (userEvent.ID < MainController.EventsViewModel.UserEventList.Count)
            {
                MainController.EventsViewModel.UserEventList.Move(userEvent.ID - 1, userEvent.ID);
            }

            RefreshIndexes();
        }

        #endregion

    }
}
