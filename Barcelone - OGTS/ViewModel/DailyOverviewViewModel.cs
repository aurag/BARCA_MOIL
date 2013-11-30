using Barcelone___OGTS.Common;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Barcelone___OGTS.Model;
using System.Windows.Data;
using System.ComponentModel;
using Npgsql;

namespace Barcelone___OGTS.ViewModel
{
    class DailyOverviewViewModel : BaseViewModel
    {
         #region Commandes
        public ICommand BackCommand { get; set; }
        #endregion

        #region Properties
        private String _dayOffTypeId;

        private String _daysLeft;

        public String DaysLeft
        {
            get { return _daysLeft; }
            set { _daysLeft = value; OnPropertyChanged("DaysLeft"); }
        }
        private String _periodStartDate;

        public String PeriodStartDate
        {
            get { return _periodStartDate; }
            set { _periodStartDate = value; OnPropertyChanged("PeriodStartDate"); }
        }
        private String _nbDays;

        public String NbDays
        {
            get { return _nbDays; }
            set { _nbDays = value; OnPropertyChanged("NbDays"); }
        }
        private String _dayOffType;

        public String DayOffType
        {
            get { return _dayOffType; }
            set { _dayOffType = value; this.OnPropertyChanged("DayOffType"); }
        }
        private String _dayOffLabel;

        public String DayOffLabel
        {
            get { return _dayOffLabel; }
            set { _dayOffLabel = value; this.OnPropertyChanged("DayOffLabel"); }
        }

        private String _selectedLeaveType;

        public String SelectedLeaveType
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

        private List<String> _leaveTypes = new List<String>();


        public List<String> LeaveTypes
        {
            get { return _leaveTypes; }
            set
            {
                _leaveTypes = value;
               OnPropertyChanged("LeaveTypes");
            }
        }

        private String _daysUsed;

        public String DaysUsed
        {
            get { return _daysUsed; }
            set { _daysUsed = value; OnPropertyChanged("DaysUsed"); }
        }

        public ICollectionView leaveRequests { get; private set; }

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
                    String tmp = result[0].ToString();
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

            // Creation de la liste de demandes de congés pour les tableaux.
            CreateLeaveRequestList();
        }

        // Chargé de la mise à jour des champs de la page
        private void UpdateFields()
        {
            UpdateDayOffType();
            UpdateUserRelatedFields();
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
                String employeeId = UserSession.Instance.User.Employee.EmployeeId;
                // date en dur, todo : récupérer la date selectionnée
                String DateSelected = "2013-04-08";
                int totalDays = 0;
                bool first = true;
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL("select start_date, end_date from public.dayoff " + "where id_employee = " + employeeId +
                                                                      " and id_day_off_type = " + _dayOffTypeId + " and (start_date - date '" + DateSelected + "' < 0) ORDER BY start_date;");
                if (result != null)
                {
                    while (result.Read())
                    {
                        if (first)
                        {
                            PeriodStartDate = result[0].ToString().Substring(0, 10);
                            first = false;
                        }
                        String startDate = result[0].ToString().Substring(0, 10);
                        String endDate = result[1].ToString().Substring(0, 10);
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
        private int ComputeNbDays(String _startDate, String _endDate)
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
            /*
            var _leaveRequests = new List<DayOff>
                {
                    new DayOff("02/04/13", "11/04/13", "01", "23/03/13", "Congé principal", "En attente", "", "", "12/03/13"),
                    new DayOff("12/04/13", "15/04/13", "01", "23/03/13", "Congé principal", "En attente", "", "", "15/03/13"),
                    new DayOff("24/04/13", "28/04/13", "01", "23/03/13", "Congé principal", "En attente", "", "", "19/03/13")
                };

            leaveRequests = CollectionViewSource.GetDefaultView(_leaveRequests);
             */
        }
    }
}
