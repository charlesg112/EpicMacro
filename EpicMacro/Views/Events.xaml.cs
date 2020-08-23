using EpicMacro.Models;
using System.Diagnostics;
using System.Windows.Controls;

namespace EpicMacro.Views
{
    /// <summary>
    /// Logique d'interaction pour Events.xaml
    /// </summary>
    public partial class Events : UserControl
    {
        public Events()
        {
            InitializeComponent();
            this.DataContext = MainController.EventsViewModel;
            MainController.EventsViewModel.UserEventList.Add(new Models.UserEvent(Models.UserEventType.Click));
            MainController.EventsViewModel.UserEventList.Add(new Models.UserEvent(Models.UserEventType.Delay));
            this.UserEventListBox.ItemsSource = MainController.EventsViewModel.UserEventList;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserEvent currentEvent = (UserEvent)((ComboBox)sender).DataContext;
            currentEvent.UpdateValues();
        }
    }
}
