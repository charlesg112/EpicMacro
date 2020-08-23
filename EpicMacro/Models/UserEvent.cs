using EpicMacro.Commands;
using EpicMacro.Exceptions;
using EpicMacro.Res;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
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
        public static int StaticID;
        private int _ID;
        public int ID { get { return _ID; } set { _ID = value; OnPropertyChanged("ID"); } }
        private UserEventType _EventType;
        private SelectableUserEvent _SelectableUserEvent;
        private string _ClickValueFieldVisibility, _KeyPressValueFieldVisibility, _DelayValueFieldVisibility, _LongClickValueFieldVisibility, _IterationsValueFieldVisibility;
        private string _KeyPressValueField;
        private float _DelayValueField;
        private int _LongClickDurationValueField;
        private int _ClickXValueField, _ClickYValueField, _LongClickXValueField, _LongClickYValueField;
        private int _IterationsValueField;

        // Looping data
        private int _IdentationLevel;
        private List<UserEvent> _LoopContents = new List<UserEvent>();
        private int _LoopEndIndex;

        // Public attributes
        public UserEventType EventType { get { return _EventType; } set { _EventType = value; OnPropertyChanged("EventType"); } }
        public string ClickValueFieldVisibility { get { return _ClickValueFieldVisibility; } set { _ClickValueFieldVisibility = value; OnPropertyChanged("ClickValueFieldVisibility"); } }
        public string KeyPressValueFieldVisibility { get { return _KeyPressValueFieldVisibility; } set { _KeyPressValueFieldVisibility = value; OnPropertyChanged("KeyPressValueFieldVisibility"); } }
        public string DelayValueFieldVisibility { get { return _DelayValueFieldVisibility; } set { _DelayValueFieldVisibility = value; OnPropertyChanged("DelayValueFieldVisibility"); } }
        public string LongClickValueFieldVisibility { get { return _LongClickValueFieldVisibility; } set { _LongClickValueFieldVisibility = value; OnPropertyChanged("LongClickValueFieldVisibility"); } }
        public string IterationsValueFieldVisibility { get { return _IterationsValueFieldVisibility; } set { _IterationsValueFieldVisibility = value; OnPropertyChanged("IterationsValueFieldVisibility"); } }
        public string KeyPressValueField { get { return _KeyPressValueField; } set { _KeyPressValueField = value; OnPropertyChanged("KeyPressValueField"); } }
        public float DelayValueField { get { return _DelayValueField; } set { _DelayValueField = value; OnPropertyChanged("DelayValueField"); } }
        public int LongClickDurationValueField { get { return _LongClickDurationValueField; } set { _LongClickDurationValueField = value; OnPropertyChanged("LongClickDurationValueField"); } }
        public int ClickXValueField { get { return _ClickXValueField; } set { _ClickXValueField = value; OnPropertyChanged("ClickXValueField"); } }
        public int ClickYValueField { get { return _ClickYValueField; } set { _ClickYValueField = value; OnPropertyChanged("ClickYValueField"); } }
        public int LongClickXValueField { get { return _LongClickXValueField; } set { _LongClickXValueField = value; OnPropertyChanged("LongClickXValueField"); } }
        public int LongClickYValueField { get { return _LongClickYValueField; } set { _LongClickYValueField = value; OnPropertyChanged("LongClickYValueField"); } }
        public int IterationsValueField { get { return _IterationsValueField; } set { _IterationsValueField = value; OnPropertyChanged("IterationsValueField"); } }
        public int IdentationLevel { get { return _IdentationLevel; } set { _IdentationLevel = value; OnPropertyChanged("IdentationLevel"); } }
        public List<UserEvent> LoopContents { get { return _LoopContents; } set { _LoopContents = value; OnPropertyChanged("LoopContents"); } }
        public int LoopEndIndex { get { return _LoopEndIndex; } set { _LoopEndIndex = value; OnPropertyChanged("LoopEndIndex"); } }
        public SelectableUserEvent SelectableUserEvent { get { return _SelectableUserEvent; } set { _SelectableUserEvent = value; OnPropertyChanged("SelectableUserEvent"); } }
        public ICommand RemoveCommand { get; private set; }
        public UserEvent(UserEventType userEventType)
        {
            EventType = userEventType;
            RemoveCommand = new RemoveEventCommand(this);
            SelectableUserEvent = MainController.EventsViewModel.UserEventComboBoxItems[UserEventValueConverter.GetComboBoxID(userEventType)];
            StaticID++;
            ID = StaticID;
            SetVisibilities();
        }

        public UserEvent(UserEventType userEventType, int id)
        {
            EventType = userEventType;
            RemoveCommand = new RemoveEventCommand(this);
            SelectableUserEvent = new SelectableUserEvent(userEventType);
            ID = id;
            SetVisibilities();
        }

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

    }
}
