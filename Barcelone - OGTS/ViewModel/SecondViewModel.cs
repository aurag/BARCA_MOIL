using Barcelone___OGTS.Common;
using Barcelone___OGTS.Model;
using Barcelone___OGTS.View;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Npgsql;
using System;
using System.Threading;

namespace Barcelone___OGTS.ViewModel
{
    public class SecondViewModel : BaseViewModel
    {
        #region Commandes
        public ICommand BackCommand { get; set; }
        public ICommand ChangePassword { get; set; }
        public ICommand AddInCET { get; set; }
        public ICommand GoToDailyOverview { get; set; }
        public ICommand LeaveRequest { get; set; }
        public ICommand GoToRequestAndProjection { get; set; }
        public ICommand GoToRHOperations { get; set; }
        public ICommand GoToPlanning { get; set; }
        public ICommand GoToCETAccount { get; set; }
        public ICommand OperationsHistory { get; set; }
        public ICommand HandleCheckBox { get; set; }
        
        #endregion

        #region Properties

        private ICollectionView _daysOff;

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
        public ICollectionView leaveRequestsOk { get; private set; }
        public ICollectionView leaveRequestsFutur { get; private set; }
        #endregion

        #region Constructor

        /// <summary>
        /// constructeur
        /// </summary>
        public SecondViewModel()
        {
            BackCommand = new Command(param => Back(), param => true);
            ChangePassword = new Command(param => PushChangePassword(), param => true);
            AddInCET = new Command(param => PushAddInCET(), param => true);
            GoToDailyOverview = new Command(param => PushDailyOverview(), param => true);
            LeaveRequest = new Command(param => PushLeaveRequest(), param => true);
            GoToRequestAndProjection = new Command(param => PushRequestAndProjection(), param => true);
            GoToRHOperations = new Command(param => PushRHOperations(), param => true);
            GoToPlanning = new Command(param => PushPlanning(), param => true);
            GoToCETAccount = new Command(param => PushCETAccount(), param => true);
            OperationsHistory = new Command(param => PushOperationsHistory(), param => true);
            HandleCheckBox = new Command(param => FirstHandleCheckBox(), param => true);


            // Creation de la liste de demandes de congés pour les tableaux.
            DbHandler.Instance.OpenConnection();
            CreateLeaveRequestList();
            CreateLeaveRequestListOk();
            CreateLeaveRequestListFutur();
            DbHandler.Instance.CloseConnection();
        }
        #endregion
        
        #region Commands Methods

        private void FirstHandleCheckBox()
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

        private void PushRHOperations()
        {
            Switcher.Switch(new RHOperations());
        }

        private void PushRequestAndProjection()
        {
            Switcher.Switch(new RequestAndProjectionView());
        }

        private void PushPlanning()
        {
            Switcher.Switch(new PlanningView());
        }
       
        private void PushOperationsHistory()
        {
            Switcher.Switch(new OperationsHistoryView());
        }

        private void PushCETAccount()
        {
            Switcher.Switch(new CETAccountView());
        }

        #endregion

        #region Days off display Methods

        /// <summary>
        /// Créé la liste des congés en attente
        /// </summary>
        private void CreateLeaveRequestList()
        {
            try
            {
                // join on id_day_off_type to get the type and the label of the day off type.
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL(@"select start_date, end_date, submission_date, type, title, status, 
                                                                   employee_commentary, superior_commentary, validation_date
                                                                   from public.dayoff, public.dayofftype
                                                                   WHERE public.dayoff.id_day_off_type = public.dayofftype.id_day_off_type;");
                List<DayOff> _daysOff = new List<DayOff>();
                while (result.Read())
                {
                    DayOff dayOff = new DayOff()
                    {
                        StartDate = result[0].ToString().Substring(0, 10),
                        EndDate = result[1].ToString().Substring(0, 10),
                        CreationDate = DateTime.Today.ToShortDateString(),
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
                        new DayOff("12/04/13", "15/04/13", "23/03/13", "02", "Congé d'ancienneté", "En attente", "Mariage de mon fils", "", "12/03/13"),
                        new DayOff("24/04/13", "28/04/13", "23/03/13", "04", "Repos forfait", "En attente", "", "", "12/03/13")
                    };
                 * */

                DaysOff = CollectionViewSource.GetDefaultView(_daysOff);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Todo : Creates the list of day off requests already accepted
        /// </summary>
        private void CreateLeaveRequestListOk()
        {
            /*
            var _leaveRequestsOk = new List<DayOff>
                {
                    new DayOff("02/02/13", "08/02/13", "23/01/13", "01", "Congé principal", "Accepté", "", "", "12/01/13"),
                    new DayOff("10/03/13", "11/03/13", "23/01/13", "01", "Congé principal", "Refusé", "", "Réunion d'équipe le 10", "12/01/13"),
                };

            leaveRequestsOk = CollectionViewSource.GetDefaultView(_leaveRequestsOk);
             */
        }

        /// <summary>
        /// Todo : Creates the list of day off previsions
        /// </summary>
        private void CreateLeaveRequestListFutur()
        {
            /*
            var _leaveRequestsFutur = new List<DayOff>
                {
                    new DayOff("02/08/13", "21/08/13", "23/03/13", "01", "Congé principal", "", "", "", ""),
                    new DayOff("15/12/13", "08/01/14", "23/03/13", "01", "Congé principal", "", "", "", ""),
                };

            leaveRequestsFutur = CollectionViewSource.GetDefaultView(_leaveRequestsFutur);
             */
        }
        #endregion

    }
}
