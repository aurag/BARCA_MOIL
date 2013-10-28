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
        public ICommand RemoveCheckBox { get; set; }

        #endregion

        #region Fields

        private ICollectionView _daysOff;
        public ICollectionView leaveRequests { get; private set; }
        public ICollectionView leaveRequestsOk { get; private set; }
        public ICollectionView leaveRequestsFutur { get; private set; }

        #endregion

        #region Properties
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
            RemoveCheckBox = new Command(param => RemoveHandleCheckBox(), param => true);
            
            
            // Creation de la liste de demandes de congés pour les tableaux.
            DbHandler.Instance.OpenConnection();
            CreateLeaveRequestList();
            CreateLeaveRequestListFutur();
            DbHandler.Instance.CloseConnection();
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
                        DbHandler.Instance.ExecSQL(String.Format(@"DELETE FROM dayoff 
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

        #endregion

        /// <summary>
        /// Créé la liste des congés en attente
        /// </summary>
        private void CreateLeaveRequestList()
        {
            // join on id_day_off_type to get the type and the label of the day off type.
            NpgsqlDataReader result = DbHandler.Instance.ExecSQL(@"select start_date, end_date, creation_date, type, title, status, 
                                                                   employee_commentary, superior_commentary, validation_date
                                                                   from public.dayoff, public.dayofftype
                                                                   WHERE public.dayoff.id_day_off_type = public.dayofftype.id_day_off_type;");
            List<DayOff> _daysOff = new List<DayOff>();
            while (result.Read())
            {
                /*
                DayOff dayOff = new DayOff(result[0].ToString().Substring(0, 10), result[1].ToString().Substring(0, 10), 
                 *                         result[2].ToString().Substring(0, 10), result[3].ToString(), result[4].ToString(), result[5].ToString(),
                                           result[6].ToString(), result[7].ToString(), result[8].ToString());
                 */

                DayOff dayOff = new DayOff()
                { 
                    StartDate = result[0].ToString().Substring(0, 10),
                    EndDate = result[1].ToString().Substring(0, 10),
                    CreationDate = result[2].ToString().Substring(0, 10),
                    Type = result[3].ToString(),
                    Title = result[4].ToString(),
                    Status = result[5].ToString(),
                    CommentSal = result[6].ToString(),
                    CommentRh = result[7].ToString(),
                    DateRh = result[8].ToString(),
                    // TODO : IdEmployee should be mapped to the current user id. 
                    IdEmployee = "201301"
                };
                _daysOff.Add(dayOff);
            }

            /*
            var _leaveRequests = new List<DayOff>
                {
                    new DayOff("02/04/13", "11/04/13", "23/03/13", "01", "Congé principal", "En attente", "", "", "12/03/13"),
                    new DayOff("12/04/13", "15/04/13", "23/03/13", "02", "Congé d'ancienneté", "En attente", "", "", "12/03/13"),
                    new DayOff("24/04/13", "28/04/13", "23/03/13", "01", "Congé principal", "En attente", "", "", "12/03/13")
                };
            */

            DaysOff = CollectionViewSource.GetDefaultView(_daysOff);
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
