using Barcelone___OGTS.Common;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Barcelone___OGTS.Model;
using Barcelone___OGTS.View;
using System.Windows;
using Npgsql;

namespace Barcelone___OGTS.ViewModel
{
    public class LeaveRequestViewModel : BaseViewModel
    {
        
        #region Commandes
        public ICommand BackCommand { get; set; }
        public ICommand CreateDayOffRequestCommand { get; set; }
        #endregion

        #region Fields
        private List<String> _leaveTypes = new List<String>();
        String _startDate = DateTime.Today.Date.ToShortDateString();
        String _endDate = DateTime.Today.Date.ToShortDateString();
        String _comment = "";
        String _nbDays = "0";
        String _isCorrect = "Oui";
        private String _selectedLeaveType;

        #endregion

        #region Properties

        public String StartDate
        {
            get { return _startDate; }
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    CheckIfRequestIsCorrect();
                    OnPropertyChanged("StartDate");
                }
            }
        }

        public List<String> LeaveTypes
        {
            get { return _leaveTypes; }
            set
            {
                _leaveTypes = value;
                this.OnPropertyChanged("LeaveTypes");
            }
        }

        public String NbDays
        {
            get 
            {
                ComputeNbDays();
                return _nbDays; 
            }
            set
            {
                if (_nbDays != value)
                {
                    _nbDays = value;
                    OnPropertyChanged("NbDays");
                }
            }
        }

        public String Comment
        {
            get { return _comment; }
            set
            {
                if (_comment != value)
                {
                    _comment = value;
                    CheckIfRequestIsCorrect();
                    OnPropertyChanged("Comment");
                }
            }
        }

        public String EndDate
        {
            get { return _endDate; }
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    CheckIfRequestIsCorrect();
                    OnPropertyChanged("EndDate");
                }
            }
        }

        public String IsCorrect
        {
            get { return _isCorrect; }
            set
            {
                if (_isCorrect != value)
                {
                    _isCorrect = value;
                    OnPropertyChanged("IsCorrect");
                }
            }
        }

        public String SelectedLeaveType
        {
            get { return _selectedLeaveType; }
            set
            {
                if (_selectedLeaveType != value)
                {
                    _selectedLeaveType = value;
                    OnPropertyChanged("SelectedLeaveType");
                }
            }
        }
        #endregion 

        #region constructor
        /// <summary>
        /// constructeur
        /// </summary>

        public LeaveRequestViewModel()
        {
            BackCommand = new Command(param => Back(), param => true);
            CreateDayOffRequestCommand = new Command(param => CreateDayOffRequest(), param => true);


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
        }
        #endregion

        private void CheckIfRequestIsCorrect()
        {
            DateTime startDate;
            DateTime endDate;
            Boolean isStartDateOk = false;
            Boolean isEndDateOk = false;
            try
            {
                startDate = Convert.ToDateTime(StartDate);
                isStartDateOk = true;
                endDate = Convert.ToDateTime(EndDate);
                isEndDateOk = true;
                IsCorrect = "Oui";
                
                // The date format is ok, we can continue
                DbHandler.Instance.OpenConnection();
    
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL(String.Format("SELECT start_date, end_date FROM public.dayoff WHERE id_employee = {0};", UserSession.Instance.User.Employee.EmployeeId));
                if (result != null)
                {
                    while (result.Read())
                    {
                        DateTime tmpStartDate = DateTime.Parse(result[0].ToString().Substring(0, 10));
                        DateTime tmpEndDate = DateTime.Parse(result[1].ToString().Substring(0, 10));
                        if ((tmpStartDate <= DateTime.Parse(EndDate) && (tmpEndDate >= DateTime.Parse(StartDate))))
                        {
                            IsCorrect = "Non";
                            NbDays = "0";
                            MessageBox.Show("Vous avez déjà une demande de congés sur ces dates", "Erreur");
                            DbHandler.Instance.CloseConnection();
                            return;
                        }
                    }
                }
                DbHandler.Instance.CloseConnection();


                ComputeNbDays();
                if (NbDays.Equals("0"))
                {
                    IsCorrect = "Non";
                    MessageBox.Show("Cette demande de congés concerne 0 jours ouvrés. \nVérifiez les dates de début et de fin de votre demande.", "Erreur");
                }
            }
            catch (FormatException)
            {
                if (!isStartDateOk)
                {
                    IsCorrect = "Non";
                    MessageBox.Show("Date de début invalide.", "Erreur");
                }
                else
                {
                    if (!isEndDateOk)
                    {
                        IsCorrect = "Non";
                        MessageBox.Show("Date de fin invalide.", "Erreur");
                    }
                }
            }
            catch (NpgsqlException)
            {
                IsCorrect = "Non";
                MessageBox.Show("Erreur de la base de données.", "Erreur");
            }
        }

        /// <summary>
        /// This function computes the number of working days between 2 dates (handles only week-ends for now.)
        /// We should have a list of days where the company is closed and handle them.
        /// </summary>
        private void ComputeNbDays()
        {
            if(!IsCorrect.Equals("Oui"))
                return;

            DateTime startDate = Convert.ToDateTime(StartDate);
            DateTime endDate = Convert.ToDateTime(EndDate);
            int nbDaysTmp = 0;
            if (startDate == endDate && endDate.DayOfWeek != DayOfWeek.Sunday && endDate.DayOfWeek != DayOfWeek.Saturday)
                NbDays = "1";
            else
            {
                if (endDate < startDate)
                    NbDays = "0";
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
                    NbDays = nbDaysTmp.ToString();
                }
            }
        }

        #region Commands Methods
        private void Back()
        {
            Switcher.SwitchBack();
        }

        /// <summary>
        /// This function creates the day off request. 
        /// </summary>
        private void CreateDayOffRequest()
        {
            try
            {
                //Error checking
                CheckIfRequestIsCorrect();
                if (!IsCorrect.Equals("Oui"))
                    return;
  
                DbHandler.Instance.OpenConnection();
                int index = LeaveTypes.IndexOf(SelectedLeaveType);
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL(String.Format("SELECT id_day_off_type FROM public.dayofftype;"));
                int i = 0;
                String selectedLeaveTypeId = "";
                while (result.Read())
                {
                    if (i == index)
                    {
                        selectedLeaveTypeId = result[0].ToString();
                        break;
                    }
                    i++;
                }
                DbHandler.Instance.CloseConnection();

                if (selectedLeaveTypeId.Equals(""))
                    Console.WriteLine("Error : the type of day off was not found");

                DbHandler.Instance.OpenConnection();

                // Le status est défaut à 2 (En attente de validation) pour le moment
                // Il faudrait peut être le mettre à 1 (Créé) puis que l'utilisateur le confirme pour qu'il passe au status 2
                int status = 2;

                String query = String.Format(@"INSERT INTO public.dayoff(id_employee, creation_date, status, id_day_off_type,
                                                       start_date, end_date, nb_days, employee_commentary)
                                                       VALUES({0}, date '{1}', {2}, {3}, date '{4}', date '{5}', {6}, '{7}');",
                                                           UserSession.Instance.User.Employee.EmployeeId, DateTime.Today.Date.ToShortDateString(), status, selectedLeaveTypeId, StartDate, EndDate, NbDays, Comment);

                // Be careful : the status is hard coded to 1 for now 
                DbHandler.Instance.ExecSQL(query);
                DbHandler.Instance.CloseConnection();
                // I'm not sure this is the best way to update the list in the main page but...
                Switcher.Switch(new HomeView());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        #endregion

    }
}
