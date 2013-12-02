using Barcelone___OGTS.Common;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Barcelone___OGTS.Model;
using System.Windows.Data;
using System.ComponentModel;
using Npgsql;
using System.Globalization;

namespace Barcelone___OGTS.ViewModel
{
    class DailyOverviewViewModel : BaseViewModel
    {
         #region Commandes
        public ICommand BackCommand { get; set; }
        #endregion

        #region Properties
        private string _dayOffTypeId;

        private string _daysLeft;

        public string DaysLeft
        {
            get { return _daysLeft; }
            set { _daysLeft = value; OnPropertyChanged("DaysLeft"); }
        }
        private string _periodStartDate;

        public string PeriodStartDate
        {
            get { return _periodStartDate; }
            set { _periodStartDate = value; OnPropertyChanged("PeriodStartDate"); }
        }

        private string _periodEndDate;

        public string PeriodEndDate
        {
            get { return _periodEndDate; }
            set { _periodEndDate = value; OnPropertyChanged("PeriodEndDate"); }
        }
        private string _nbDays;

        public string NbDays
        {
            get { return _nbDays; }
            set { _nbDays = value; OnPropertyChanged("NbDays"); }
        }
        private string _dayOffType;

        public string DayOffType
        {
            get { return _dayOffType; }
            set { _dayOffType = value; this.OnPropertyChanged("DayOffType"); }
        }
        private string _dayOffLabel;

        public string DayOffLabel
        {
            get { return _dayOffLabel; }
            set { _dayOffLabel = value; this.OnPropertyChanged("DayOffLabel"); }
        }

        private string _selectedLeaveType;

        public string SelectedLeaveType
        {
            get { return _selectedLeaveType; }
            set
            {
                if (_selectedLeaveType != value)
                {
                    _selectedLeaveType = value;
                    OnPropertyChanged("SelectedLeaveType");
                    UpdateFields();
                }
            }
        }

        private List<string> _leaveTypes = new List<string>();


        public List<string> LeaveTypes
        {
            get { return _leaveTypes; }
            set
            {
                _leaveTypes = value;
               OnPropertyChanged("LeaveTypes");
            }
        }

        private string _daysUsed;

        public string DaysUsed
        {
            get { return _daysUsed; }
            set { _daysUsed = value; OnPropertyChanged("DaysUsed"); }
        }

        private DateTime _selectedDate;

        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    UpdateUserRelatedFields();
                    OnPropertyChanged("SelectedDate");
                    OnPropertyChanged("DisplayDate");
                }
            }
        }

        public string DisplayDate
        {
            get 
            {
                return SelectedDate.ToString("D", CultureInfo.CreateSpecificCulture("fr-FR")); 
            }
        }

        private ICollectionView _daysOffList;
        public ICollectionView DaysOffList {
            get
            {
                return _daysOffList;
            }
            set
            {
                _daysOffList = value;
                OnPropertyChanged("DaysOffList");
            }
        }

        #endregion

        /// <summary>
        /// constructeur
        /// </summary>
        public DailyOverviewViewModel()
        {
            BackCommand = new Command(param => Back(), param => true);

            // Création de la liste des types de congés
            DbHandler.Instance.OpenConnection();
            NpgsqlDataReader result = DbHandler.Instance.ExecSQL("select title from public.dayofftype;");
            if (result != null)
            {
                while (result.Read())
                {
                    string tmp = result[0].ToString();
                    _leaveTypes.Add(tmp);
                }
            }
            DbHandler.Instance.CloseConnection();
            
            try
            {
                SelectedLeaveType = LeaveTypes[0];
            }
            catch (Exception e)
            {
                Console.WriteLine("Liste des types de congés vide : " + e.Message);
            }
            SelectedDate = DateTime.Today;
        }

        // Chargé de la mise à jour des champs de la page
        private void UpdateFields()
        {
            Console.WriteLine("Appel");
            UpdateDayOffType();
            UpdateUserRelatedFields();
            CreateLeaveRequestList();
        }

        private void UpdateDayOffType()
        {
            DbHandler.Instance.OpenConnection();
            try
            {
                int index = LeaveTypes.IndexOf(SelectedLeaveType);
                int i = 0;
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL("select type, title, nb_days, id_day_off_type from public.dayofftype;");
                if (result != null)
                {
                    while (result.Read())
                    {
                        if (i == index)
                        {
                            DayOffType = result[0].ToString();
                            DayOffLabel = result[1].ToString();

                            // TODO : gestion de l'ancienneté
                            if (SelectedLeaveType.Equals("Congés d'ancienneté"))
                                NbDays = "0";
                            else
                                NbDays = result[2].ToString();
                            _dayOffTypeId = result[3].ToString();
                            break;
                        }
                        i++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur mise à jour situation du jour : " + e.Message);
            }
            finally
            {
                DbHandler.Instance.CloseConnection();
            }
        }

        private void UpdateUserRelatedFields()
        {
            DbHandler.Instance.OpenConnection();
            try
            {
                string employeeId = UserSession.Instance.User.Employee.EmployeeId;
                int totalDays = 0;
                bool first = true;
                if (SelectedDate == null)
                    SelectedDate = DateTime.Today;
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL("select start_date, end_date from public.dayoff " + "where id_employee = " + employeeId +
                                                                      " and id_day_off_type = " + _dayOffTypeId + " and (start_date - date '" + SelectedDate.ToShortDateString() + "' < 0) ORDER BY start_date;");
                if (result != null)
                {
                    while (result.Read())
                    {
                        if (first)
                        {
                            PeriodStartDate = result[0].ToString().Substring(0, 10);
                            first = false;
                        }
                        string startDate = result[0].ToString().Substring(0, 10);
                        string endDate = result[1].ToString().Substring(0, 10);
                        int daysForLeaveRequest = ComputeNbDays(startDate, endDate);
                        totalDays += daysForLeaveRequest;
                    }
                }

                DaysUsed = totalDays.ToString();
                DaysLeft = (int.Parse(NbDays) - totalDays).ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur mise à jour situation du jour : " + e.Message);
            }
            finally
            {
                DbHandler.Instance.CloseConnection();
            }
        }


        /// <summary>
        /// This function computes the number of working days between 2 dates (handles only week-ends for now.)
        /// We should have a list of days where the company is closed and handle them.
        /// </summary>
        private int ComputeNbDays(string _startDate, string _endDate)
        {
            DateTime startDate = Convert.ToDateTime(_startDate);
            DateTime endDate = Convert.ToDateTime(_endDate);
            int nbDaysTmp = 0;
            int nbDaysRes = 0;
            if (startDate == endDate && endDate.DayOfWeek != DayOfWeek.Sunday && endDate.DayOfWeek != DayOfWeek.Saturday)
                nbDaysRes = 1;
            else
            {
                if (endDate < startDate)
                    nbDaysRes = 0;
                else
                {
                    // Put the last day on a Friday
                    while (endDate > startDate && endDate.DayOfWeek != DayOfWeek.Sunday && endDate.DayOfWeek != DayOfWeek.Saturday)
                    {
                        endDate = endDate.AddDays(-1);
                        nbDaysTmp++;
                    }

                    // Less than a week difference bewteen the 2 days
                    if (endDate == startDate)
                        nbDaysTmp++;

                    // Put the first day on a Monday
                    while (startDate < endDate && startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        startDate = startDate.AddDays(1);
                        nbDaysTmp++;
                    }

                    // Compute the number of days between the 2 new dates
                    TimeSpan totalNbDaysSpan = endDate - startDate;

                    // 5 working day in a week * number of weeks (number of days / 7)
                    nbDaysTmp += 5 * (totalNbDaysSpan.Days / 7);
                    nbDaysRes = nbDaysTmp;
                }
            }

            return nbDaysRes;
        }

        /// <summary>
        /// réponse à la commande back
        /// </summary>
        private void Back()
        {
            Switcher.SwitchBack();
        }

        /// <summary>
        /// Todo : Creates the liste of days off request waiting for validation
        /// </summary>
        private void CreateLeaveRequestList()
        {

            DbHandler.Instance.OpenConnection();

            NpgsqlDataReader result = DbHandler.Instance.ExecSQL(string.Format(@"select start_date, end_date, creation_date, type, title, status, 
                                                                   employee_commentary, superior_commentary, validation_date
                                                                   from public.dayoff, public.dayofftype
                                                                   WHERE public.dayoff.id_day_off_type = public.dayofftype.id_day_off_type
                                                                   AND public.dayoff.id_employee={0} AND public.dayoff.id_day_off_type = " + _dayOffTypeId + " ORDER BY start_date;", UserSession.Instance.User.Employee.EmployeeId));
            List<DayOff> _daysOff = null;
            if (result != null)
            {
                _daysOff = new List<DayOff>();
                if (!result.HasRows)
                {
                    PeriodStartDate = "Pas de congés";
                    PeriodEndDate = "Pas de congés";
                }
                while (result.Read())
                {
                    DayOff dayOff = new DayOff()
                    {
                        StartDate = result[0].ToString().Substring(0, 10),
                        EndDate = result[1].ToString().Substring(0, 10),
                        CreationDate = DateTime.Today.ToShortDateString(),
                        Type = result[3].ToString(),
                        Name = result[4].ToString(),
                        Status = result[5].ToString(),
                        CommentSal = result[6].ToString(),
                        CommentRh = result[7].ToString(),
                        DateRh = result[8].ToString(),
                        IdEmployee = UserSession.Instance.User.Employee.EmployeeId
                    };

                    if (result[1].ToString() != "")
                    {
                        PeriodEndDate = result[1].ToString().Substring(0, 10);

                        if (DateTime.Parse(result[1].ToString().Substring(0, 10)) > DateTime.Parse(PeriodEndDate))
                            PeriodEndDate = result[1].ToString().Substring(0, 10);
                    }

                    _daysOff.Add(dayOff);
                }
            }

            DbHandler.Instance.CloseConnection();

            DaysOffList = CollectionViewSource.GetDefaultView(_daysOff);
        }
    }
}
