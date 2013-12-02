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
    public class LeaveRequestForecastViewModel : BaseViewModel
    {
        
        #region Commandes
        public ICommand BackCommand { get; set; }
        public ICommand CreateDayOffRequestCommand { get; set; }
        #endregion

        #region Fields
        private List<string> _leaveTypes = new List<string>();
        static string _startDate = DateTime.Today.Date.ToShortDateString();
        static string _endDate = DateTime.Today.Date.ToShortDateString();
        string _comment = "";
        string _nbDays = "0";

        #endregion

        #region Properties

        public static string StartDate
        {
            get { return _startDate; }
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                }
            }
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

        public static string EndDate
        {
            get { return _endDate; }
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                }
            }
        }

        #endregion 

        #region constructor
        /// <summary>
        /// constructeur
        /// </summary>

        public LeaveRequestForecastViewModel()
        {
            BackCommand = new Command(param => Back(), param => true);
            CreateDayOffRequestCommand = new Command(param => CreateDayOffRequest(), param => true);   
        }
        #endregion

        private Boolean CheckIfRequestIsCorrect()
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
                            NbDays = "0";
                            MessageBox.Show("Vous avez déjà une demande de congés sur ces dates", "Erreur");
                            DbHandler.Instance.CloseConnection();
                            return false;
                        }
                    }
                }
                DbHandler.Instance.CloseConnection();


                ComputeNbDays();
                if (NbDays.Equals("0"))
                {
                    MessageBox.Show("Cette demande de congés concerne 0 jours ouvrés. \nVérifiez les dates de début et de fin de votre demande.", "Erreur");
                    return false;
                }
            }
            catch (FormatException)
            {
                if (!isStartDateOk)
                {
                    MessageBox.Show("Date de début invalide.", "Erreur");
                    return false;
                }
                else
                {
                    if (!isEndDateOk)
                    {
                        MessageBox.Show("Date de fin invalide.", "Erreur");
                        return false;
                    }
                }
            }
            catch (NpgsqlException)
            {
                MessageBox.Show("Erreur de la base de données.", "Erreur");
                return false;
            }

            return true;
        }

        /// <summary>
        /// This function computes the number of working days between 2 dates (handles only week-ends for now.)
        /// We should have a list of days where the company is closed and handle them.
        /// </summary>
        private void ComputeNbDays()
        {
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
                if (!CheckIfRequestIsCorrect())
                    return;

                DbHandler.Instance.OpenConnection();

                try
                {
                    // Todo : gérer les entrées utilisateurs proprement pour éviter les injections sql et les plantages
                    Comment = Comment.Replace("'", "''");

                    string employee_id = UserSession.Instance.User.Employee.EmployeeId;

                    string query = string.Format(@"INSERT INTO public.dayoffforecast (id_employee, creation_date, start_date, end_date, employee_commentary, nb_days)
                                                       VALUES({0}, date '{1}', date '{2}', date '{3}', '{4}', {5});",
                                                              employee_id, DateTime.Today.Date.ToShortDateString(), StartDate, EndDate, Comment, NbDays);


                    DbHandler.Instance.ExecSQL(query);
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
