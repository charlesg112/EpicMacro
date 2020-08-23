using EpicMacro.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EpicMacro.Res
{
    public class SelectableUserEvent : INotifyPropertyChanged
    {

        private string _DisplayedName;
        public UserEventType _UserEventType;
        public string DisplayedName { get { return _DisplayedName; } set { _DisplayedName = value; OnPropertyChanged("DisplayedName"); } }
        public UserEventType UserEventType { get { return _UserEventType; } set { _UserEventType = value; OnPropertyChanged("UserEventType"); } }

        public SelectableUserEvent(string displayedName)
        {
            DisplayedName = displayedName;
            UserEventType = UserEventValueConverter.GetType(displayedName);
        }

        public SelectableUserEvent(UserEventType userEventType)
        {
            UserEventType = userEventType;
            DisplayedName = UserEventValueConverter.GetString(userEventType);
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

    }
}
