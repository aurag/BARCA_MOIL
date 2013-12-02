using Barcelone___OGTS.Common;
using Barcelone___OGTS.Model;
using Barcelone___OGTS.View;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace Barcelone___OGTS.ViewModel
{
    public class RequestAndProjectionViewModel : BaseViewModel
    {
        #region Commandes
        public ICommand BackCommand { get; set; }
        public ICommand ChangePassword { get; set; }
        public ICommand AddInCET { get; set; }
        public ICommand GoToDailyOverview { get; set; }
        public ICommand GoToLeaveRequest { get; set; }
        public ICommand GoToLeaveRequestForecast { get; set; }
        public ICommand RemoveCheckBox { get; set; }

        #endregion

        #region Properties

        private ICollectionView _daysOff;

        public ICollectionView leaveRequests { get; private set; }
        public ICollectionView leaveRequestsOk { get; private set; }
        public ICollectionView leaveRequestsFutur { get; private set; }

        public ICollectionView DaysOff
        {
            get
            {
                return _daysOff;
            }
            set
            {
                _daysOff = value;
                OnPropertyChanged("DaysOff");
            }
        }
        #endregion

        /// <summary>
        /// constructeur
        /// </summary>
        public RequestAndProjectionViewModel()
        {
            BackCommand = new Command(param => Back(), param => true);
            ChangePassword = new Command(param => PushChangePassword(), param => true);
            AddInCET = new Command(param => PushAddInCET(), param => true);
            GoToDailyOverview = new Command(param => PushDailyOverview(), param => true);
            GoToLeaveRequest = new Command(param => PushLeaveRequest(), param => true);
            GoToLeaveRequestForecast = new Command(param => PushLeaveRequestForecast(), param => true);
            RemoveCheckBox = new Command(param => RemoveHandleCheckBox(), param => true);
            
            
            // Creation de la liste de demandes de congés pour les tableaux.
            CreateLeaveRequestList();
            CreateLeaveRequestListFutur();
        }

        #region Command methods

        /// <summary>
        /// This function handles the deletion of day off requests
        /// </summary>
        private void RemoveHandleCheckBox()
        {
            try
            {
                DbHandler.Instance.OpenConnection();
                for (int i = 0; i < ((List<DayOff>)DaysOff.SourceCollection).Count; i++)
                {
                    DayOff day = ((List<DayOff>)DaysOff.SourceCollection)[i];
                    if (day.IsSelected)
                    {
                        // id_employee is hard coded to 4 for now.
                        DbHandler.Instance.ExecSQL(string.Format(@"DELETE FROM dayoff 
                                                               WHERE start_date = (date '{0}') and end_date = (date '{1}') and id_employee = {2}",
                                                                   day.StartDate, day.EndDate, 4));
                        ((List<DayOff>)DaysOff.SourceCollection).Remove(day);
                        i--;
                    }
                }
                DaysOff.Refresh();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                DbHandler.Instance.CloseConnection();
            }
        }

        /// <summary>
        /// réponse à la commande back
        /// </summary>
        private void Back()
        {
            Switcher.SwitchBack();
        }
        private void PushChangePassword()
        {
            Switcher.Switch(new ChangePassword());
        }

        private void PushAddInCET()
        {
            Switcher.Switch(new AddInCET());
        }

        private void PushDailyOverview()
        {
            Switcher.Switch(new DailyOverview());
        }

        private void PushLeaveRequest()
        {
            Switcher.Switch(new LeaveRequestView());
        }

        private void PushLeaveRequestForecast()
        {
            Switcher.Switch(new LeaveRequestForecastView());
        }

        #endregion

        /// <summary>
        /// Créé la liste des congés en attente
        /// </summary>
        private void CreateLeaveRequestList()
        {
            DaysOff = CollectionViewSource.GetDefaultView(DbHandler.Instance.getDaysOffList());
        }

        /// <summary>
        /// Todo : Creates the list of days off prevision
        /// </summary>
        private void CreateLeaveRequestListFutur()
        {
            /*
            var _leaveRequestsFutur = new List<DayOff>
                {
                    new DayOff("02/08/13", "21/08/13", "01", "23/03/13", "Congé principal", "", "Vacances d'été", "", ""),
                    new DayOff("15/12/13", "08/01/14", "01", "23/03/13", "Congé principal", "", "Vacances de Noël", "", ""),
                };

            leaveRequestsFutur = CollectionViewSource.GetDefaultView(_leaveRequestsFutur);
             */
        }
    }
}
