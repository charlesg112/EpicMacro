using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EpicMacro.Res
{
    public class PopUpManager : INotifyPropertyChanged
    {

        private string _LoopCountVisibility;
        private bool _LoopCountEnabled;
        public string LoopCountVisibility { get { return _LoopCountVisibility; } set { _LoopCountVisibility = value; OnPropertyChanged("LoopCountVisibility"); } }
        public bool LoopCountEnabled { get { return _LoopCountEnabled; } set { _LoopCountEnabled = value; OnPropertyChanged("LoopCountEnabled"); } }


        public PopUpManager(bool loopEnabled)
        {
            UpdateLoopCountVisibility(loopEnabled);
            LoopCountEnabled = true;
        }

        public void UpdateLoopCountVisibility(bool loopEnabled)
        {
            if (loopEnabled) LoopCountVisibility = "Visible";
            else LoopCountVisibility = "Collapsed";
        }

        public void UpdateLoopCountEnabled(bool infinityEnabled)
        {
            if (infinityEnabled) LoopCountEnabled = false;
            else LoopCountEnabled = true;
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
