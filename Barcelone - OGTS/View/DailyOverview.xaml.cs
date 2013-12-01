using Barcelone___OGTS.Common;
using Barcelone___OGTS.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Barcelone___OGTS.View
{
    /// <summary>
    /// Logique d'interaction pour DailyOverview.xaml
    /// </summary>
    public partial class DailyOverview : UserControl, ISwitchable
    {
        public DailyOverview()
        {
            InitializeComponent();
            DataContext = new DailyOverviewViewModel();
        }

        private void CalendarLoaded(object sender, RoutedEventArgs e)
        {
            if (this.Calendar.SelectedDate.HasValue)
            {
                this.Calendar.DisplayDate = this.Calendar.SelectedDate.Value;
            }
        }

        #region ISWitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
