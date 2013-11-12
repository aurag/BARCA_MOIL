using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Barcelone___OGTS.Common;
using Barcelone___OGTS.ViewModel;
using Barcelone___OGTS.Model;

namespace Barcelone___OGTS.View
{
    /// <summary>
    /// Logique d'interaction pour HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl, ISwitchable
    {
        public HomeView()
        {
            InitializeComponent();
            DataContext = new HomeViewModel();
        }

        private void RowFilterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DayOff day = ((CheckBox)sender).Tag as DayOff;
                if (day.IsSelected)
                    day.IsSelected = false;
                else
                    day.IsSelected = true;
            }
            catch (Exception e2)
            {
                Console.WriteLine("Erreur lors suppression d'une ligne : " + e2.Message);
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
