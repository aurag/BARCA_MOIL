using Barcelone___OGTS.Common;
using Barcelone___OGTS.Model;
using Barcelone___OGTS.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
namespace Barcelone___OGTS.View
{
    /// <summary>
    /// Logique d'interaction pour Switchable_UserControl1.xaml
    /// </summary>
    public partial class RHOperations : UserControl, ISwitchable
    {
        public RHOperations()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DayOff day = ((CheckBox)sender).Tag as DayOff;
                if (day.IsSelectedOk)
                    day.IsSelectedOk = false;
                else
                    day.IsSelectedOk = true;
            }
            catch (Exception e2)
            {
                Console.WriteLine("Erreur lors sélection d'une ligne pour validation : " + e2.Message);
            }
        }

        private void Nok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DayOff day = ((CheckBox)sender).Tag as DayOff;
                if (day.IsSelectedNok)
                    day.IsSelectedNok = false;
                else
                    day.IsSelectedNok = true;
            }
            catch (Exception e2)
            {
                Console.WriteLine("Erreur lors sélection d'une ligne pour validation : " + e2.Message);
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
