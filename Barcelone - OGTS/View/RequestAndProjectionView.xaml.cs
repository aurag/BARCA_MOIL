using Barcelone___OGTS.Common;
using Barcelone___OGTS.Model;
using Barcelone___OGTS.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Barcelone___OGTS.View
{
    /// <summary>
    /// Logique d'interaction pour RequestAndProjectionView.xaml
    /// </summary>
    public partial class RequestAndProjectionView : UserControl, ISwitchable
    {
        public RequestAndProjectionView()
        {
            InitializeComponent();
            DataContext = new RequestAndProjectionViewModel();
        }

        private void RowFilterButton_Click(object sender, RoutedEventArgs e)
        {
            DayOff day = ((CheckBox)sender).Tag as DayOff;
            if (day.IsSelected)
                day.IsSelected = false;
            else
                day.IsSelected = true;
        }

        #region ISWitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
