using Barcelone___OGTS.Common;
using Barcelone___OGTS.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;


namespace Barcelone___OGTS.View
{
    /// <summary>
    /// Logique d'interaction pour LeaveRequestForecastView.xaml
    /// </summary>
    public partial class LeaveRequestForecastView : UserControl, ISwitchable
    {
        public LeaveRequestForecastView()
        {
            InitializeComponent();
        }

        #region ISWitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }
        #endregion

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime d1 = DateTime.Today;
            DateTime d2 = DateTime.Today;

            Boolean first = true;

            foreach (var d in Calendar.SelectedDates)
            {
                if (first)
                {
                    d1 = d;
                    first = false;
                }
                d2 = d;
            }

            Console.WriteLine("Début : " + d1.ToShortDateString());
            Console.WriteLine("Fin : " + d2.ToShortDateString());

            LeaveRequestForecastViewModel.StartDate = d1.ToShortDateString();
            LeaveRequestForecastViewModel.EndDate = d2.ToShortDateString();

            StartBox.Text = d1.ToShortDateString();
            EndBox.Text = d2.ToShortDateString();
        }

    }
}
