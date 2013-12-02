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
        private List<string> _leaveTypes = new List<string>();
        string _startDate = DateTime.Today.Date.ToShortDateString();
        string _endDate = DateTime.Today.Date.ToShortDateString();
        string _comment = "";
        string _nbDays = "0";
        string _isCorrect = "Oui";
        private string _selectedLeaveType;

        #endregion

        #region Properties

        public string StartDate
        {
            get { return _startDate; }
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    ComputeNbDays();
                    OnPropertyChanged("StartDate");
                }
            }
        }

        public List<string> LeaveTypes
        {
            get { return _leaveTypes; }
            set
            {
                _leaveTypes = value;
                this.OnPropertyChanged("LeaveTypes");
            }
        }

        private int _nbDaysMax;

        public int NbDaysMax
        {
            get { return _nbDaysMax; }
            set { _nbDaysMax = value; OnPropertyChanged("NbDaysMax"); }
        }

        public string NbDays
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

        public string Comment
        {
            get { return _comment; }
            set
            {
                if (_comment != value)
                {
                    _comment = value;
                    OnPropertyChanged("Comment");
                }
            }
        }

        public string EndDate
        {
            get { return _endDate; }
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    ComputeNbDays();
                    OnPropertyChanged("EndDate");
                }
            }
        }

        public string IsCorrect
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

        public string SelectedLeaveType
        {
            get { return _selectedLeaveType; }
            set
            {
                if (_selectedLeaveType != value)
                {
                    _selectedLeaveType = value;
                    getDaysForType();
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
    
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL(string.Format("SELECT start_date, end_date FROM public.dayoff WHERE id_employee = {0};", UserSession.Instance.User.Employee.EmployeeId));
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

                if (int.Parse(NbDays) > NbDaysMax)
                {
                    IsCorrect = "Non";
                    MessageBox.Show("Vous n'avez pas assez de jours de congés pour cette durée. Nous n'avez que " + NbDaysMax + " jours de congés disponibles pour ce type de congé.", "Erreur");
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
        /// Récupère le nombre de jours de congés disponibles pour le type de congé sélectionné
        /// </summary>
        /// <returns></returns>
        private void getDaysForType()
        {
            
            int res = 0;
            string dayTypeNumber = "";
            dayTypeNumber = getLeaveTypeNumber();

            string employeeId = UserSession.Instance.User.Employee.EmployeeId;

            DbHandler.Instance.OpenConnection();
            try
            {
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL("select days_type_" + dayTypeNumber + " from employee where id_employee =" + employeeId + ";");

                if (result != null)
                {
                    while (result.Read())
                    {
                        res = int.Parse(result[0].ToString());
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

            NbDaysMax = res;
        }

        private string getLeaveTypeNumber()
        {
            if (SelectedLeaveType.Equals("Congés légaux"))
                return "01";
            if (SelectedLeaveType.Equals("Congés d'ancienneté"))
                return "02";
            if (SelectedLeaveType.Equals("Congés supplémentaires"))
                return "03";
            if (SelectedLeaveType.Equals("Repos forfait"))
                return "04";
            if (SelectedLeaveType.Equals("Ponts et fermetures d'entreprise"))
                return "05";
            if (SelectedLeaveType.Equals("Congés sans solde"))
                return "17";
            if (SelectedLeaveType.Equals("Congés de l'année précédente"))
                return "18";
            return "";
        }


        /// <summary>
        /// This function computes the number of working days between 2 dates (handles only week-ends for now.)
        /// We should have a list of days where the company is closed and handle them.
        /// </summary>
        public void ComputeNbDays()
        {
            if (!IsCorrect.Equals("Oui"))
                return;
            try
            {
                DateTime startDate = Convert.ToDateTime(StartDate);
                DateTime endDate = Convert.ToDateTime(EndDate);
                int nbDaysTmp = 0;
                if (startDate == endDate && endDate.DayOfWeek == DayOfWeek.Sunday || endDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    IsCorrect = "Non";
                    NbDays = "0";
                }
                if (startDate == endDate && endDate.DayOfWeek != DayOfWeek.Sunday && endDate.DayOfWeek != DayOfWeek.Saturday)
                    NbDays = "1";
                else
                {
                    if (endDate < startDate)
                    {
                        IsCorrect = "Non";
                        NbDays = "0";
                    }
                    else
                    {
                        // Put the last day on a Friday
                        while (endDate > startDate && endDate.DayOfWeek != DayOfWeek.Sunday && endDate.DayOfWeek != DayOfWeek.Saturday)
                        {
                            endDate = endDate.AddDays(-1);
                            nbDaysTmp++;
                        }

                        // Less than a week difference bewteen the 2 days
                        if (endDate == startDate && endDate.DayOfWeek != DayOfWeek.Sunday && endDate.DayOfWeek != DayOfWeek.Saturday)
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
            catch (Exception e)
            {
                NbDays = "0";
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
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL(string.Format("SELECT id_day_off_type FROM public.dayofftype;"));
                int i = 0;
                string selectedLeaveTypeId = "";
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

                try
                {
                    // Todo : gérer les entrées utilisateurs proprement pour éviter les injections sql et les plantages
                    Comment = Comment.Replace("'", "''");

                    string employee_id = UserSession.Instance.User.Employee.EmployeeId;
                    // Le status est défaut à 2 (En attente de validation) pour le moment
                    // Il faudrait peut être le mettre à 1 (Créé) puis que l'utilisateur le confirme pour qu'il passe au status 
                    int status = 2;
                    string query = string.Format(@"INSERT INTO public.dayoff(id_employee, creation_date, status, id_day_off_type,
                                                       start_date, end_date, nb_days, employee_commentary)
                                                       VALUES({0}, date '{1}', {2}, {3}, date '{4}', date '{5}', {6}, '{7}');",
                                                              employee_id , DateTime.Today.Date.ToShortDateString(), status, selectedLeaveTypeId, StartDate, EndDate, NbDays, Comment);


                    DbHandler.Instance.ExecSQL(query);

                    string dayTypeNumber = "";
                    dayTypeNumber = getLeaveTypeNumber();

                    DbHandler.Instance.ExecSQL("UPDATE public.employee SET days_type_" + dayTypeNumber + " = " + (NbDaysMax - int.Parse(NbDays)).ToString() + " where id_employee = " + employee_id + ";");

                }
                catch (Exception e)
                {
                    Console.WriteLine("Erreur lors de la création du congé : " + e.Message);
                }
                finally
                {
                    DbHandler.Instance.CloseConnection();
                }
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
