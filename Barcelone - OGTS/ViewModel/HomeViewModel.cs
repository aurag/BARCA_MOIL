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
using System.Windows;

namespace Barcelone___OGTS.ViewModel
{
    public class HomeViewModel : BaseViewModel
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
        public ICommand HandleCheckBoxFutur { get; set; }
        public ICommand DisconnectCommand { get; set; }
        
        #endregion

        #region Properties

        private string _lastConnectionDate;

        public string LastConnectionDate
        {
            get 
            {
                if (UserSession.Instance.User.LastConnectionDate != _lastConnectionDate)
                    LastConnectionDate = UserSession.Instance.User.LastConnectionDate;
                return _lastConnectionDate; 
            }
            set { _lastConnectionDate = value; OnPropertyChanged("LastConnectionDate"); }
        }

        private string _lastConnectionTime;

        public string LastConnectionTime
        {
            get
            {
                if (UserSession.Instance.User.LastConnectionTime != _lastConnectionTime)
                    _lastConnectionTime = UserSession.Instance.User.LastConnectionTime;
                return _lastConnectionTime;
            }
            set { _lastConnectionTime = value; OnPropertyChanged("LastConnectionTime"); }
        }

        private Visibility _isRhVisibility;

        public Visibility IsRhVisibility
        {
            get 
            {
                return _isRhVisibility; 
            }
            set 
            { 
                _isRhVisibility = value;
                OnPropertyChanged("IsRhVisibility");
            }
        }
        
        private Boolean _isRh;

        public Boolean IsRh
        {
            get 
            {
                return _isRh; 
            }
            set 
            { 
                _isRh = value;
                OnPropertyChanged("IsRh");
            }
        }

        private ICollectionView _daysOffWaiting;

        public ICollectionView DaysOffWaiting
        {
            get
            {
                return _daysOffWaiting;
            }
            set
            {
                _daysOffWaiting = value;
                OnPropertyChanged("DaysOffWaiting");
            }
        }

        private ICollectionView _daysOffOver;

        public ICollectionView DaysOffOver
        {
            get
            {
                return _daysOffOver;
            }
            set
            {
                _daysOffOver = value;
                OnPropertyChanged("DaysOffOver");
            }
        }

        private ICollectionView _daysOffFutur;

        public ICollectionView DaysOffFutur
        {
            get
            {
                return _daysOffFutur;
            }
            set
            {
                _daysOffFutur = value;
                OnPropertyChanged("DaysOffFutur");
            }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set 
            { 
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// constructeur
        /// </summary>
        public HomeViewModel()
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
            DisconnectCommand = new Command(param => Disconnect(), param => true);
            HandleCheckBoxFutur = new Command(param => HandleFuturDaysOff(), param => true);
            
            // Initialisation de la vue
            IsRh = UserSession.Instance.User.Employee.IsRH;
            IsRhVisibility = (IsRh ? Visibility.Visible : Visibility.Hidden);

            // Creation de la liste de demandes de congés pour les tableaux.
            CreateLeaveRequestList();
            CreateLeaveRequestListOver();
            CreateLeaveRequestListFutur();


            // Récupération de la session de l'utilisateur
            Name = UserSession.Instance.User.Employee.Firstname + " " + UserSession.Instance.User.Employee.Lastname;
        }
        #endregion
        
        #region Commands Methods

        private void Disconnect()
        {
            Switcher.Switch(new LoginView());
        }

        private void FirstHandleCheckBox()
        {
            try
            {
                DbHandler.Instance.OpenConnection();
                for (int i = 0; i < ((List<DayOff>)DaysOffWaiting.SourceCollection).Count; i++)
                {
                    DayOff day = ((List<DayOff>)DaysOffWaiting.SourceCollection)[i];
                    if (day.IsSelected)
                    {
                        DbHandler.Instance.ExecSQL(string.Format(@"DELETE FROM dayoff 
                                                               WHERE start_date = (date '{0}') and end_date = (date '{1}') and id_employee = {2}",
                                                                   day.StartDate, day.EndDate, UserSession.Instance.User.Employee.EmployeeId));
                        ((List<DayOff>)DaysOffWaiting.SourceCollection).Remove(day);
                        i--;
                    }
                }
                DaysOffWaiting.Refresh();
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

        private void HandleFuturDaysOff()
        {
            try
            {
                DbHandler.Instance.OpenConnection();
                for (int i = 0; i < ((List<DayOff>)DaysOffFutur.SourceCollection).Count; i++)
                {
                    DayOff day = ((List<DayOff>)DaysOffFutur.SourceCollection)[i];
                    if (day.IsSelected)
                    {
                        DbHandler.Instance.ExecSQL(string.Format(@"DELETE FROM dayoffforecast 
                                                               WHERE start_date = (date '{0}') and end_date = (date '{1}') and id_employee = {2}",
                                                                   day.StartDate, day.EndDate, UserSession.Instance.User.Employee.EmployeeId));
                        ((List<DayOff>)DaysOffFutur.SourceCollection).Remove(day);
                        i--;
                    }
                }
                DaysOffFutur.Refresh();
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
                DaysOffWaiting = CollectionViewSource.GetDefaultView(DbHandler.Instance.getDaysOffList("2"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Creates the list of day off requests already accepted
        /// </summary>
        private void CreateLeaveRequestListOver()
        {
            try
            {
                DaysOffOver = CollectionViewSource.GetDefaultView(DbHandler.Instance.getDaysOffList("5 OR public.dayoff.status = 6"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Todo : Creates the list of day off previsions
        /// </summary>
        private void CreateLeaveRequestListFutur()
        {
            DbHandler.Instance.OpenConnection();
            List<DayOff> daysOffFutur = new List<DayOff>();
            try
            {
                string id_employee = UserSession.Instance.User.Employee.EmployeeId;
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL("select start_date, end_date, employee_commentary from dayoffforecast where id_employee = " + id_employee + ";");
                if (result != null)
                {
                    while (result.Read())
                    {
                        DayOff tmp = new DayOff {
                            StartDate = result[0].ToString().Substring(0, 10),
                            EndDate = result[1].ToString().Substring(0, 10),
                            CommentSal = result[2].ToString()
                        };
                        daysOffFutur.Add(tmp);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur : " + e.Message);
            }
            finally
            {
                DbHandler.Instance.CloseConnection();
            }

            DaysOffFutur = CollectionViewSource.GetDefaultView(daysOffFutur);

        }
        #endregion

    }
}
