﻿using Barcelone___OGTS.Common;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Barcelone___OGTS.Model;
using Barcelone___OGTS.View;
using System.Windows;
using Npgsql;

namespace Barcelone___OGTS.ViewModel
{
    public class LeaveRequestViewModelCET : BaseViewModel
    {
        
        #region Commandes
        public ICommand BackCommand { get; set; }
        public ICommand CreateDayOffRequestCommand { get; set; }
        #endregion

        #region Fields
        string _startDate = DateTime.Today.Date.ToShortDateString();
        string _endDate = DateTime.Today.Date.ToShortDateString();
        string _comment = "";
        string _nbDays = "0";
        string _isCorrect = "Oui";

        #endregion

        #region Properties

        private int _cETCurrentNumber;

        public int CETCurrentNumber
        {
            get { return _cETCurrentNumber; }
            set { _cETCurrentNumber = value; OnPropertyChanged("CETCurrentNumber"); }
        }

        public string StartDate
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
                    CheckIfRequestIsCorrect();
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

        #endregion 

        #region constructor
        /// <summary>
        /// constructeur
        /// </summary>

        public LeaveRequestViewModelCET()
        {
            BackCommand = new Command(param => Back(), param => true);
            CreateDayOffRequestCommand = new Command(param => CreateDayOffRequest(), param => true);

            // Mise à jour du solde du CET 
            DbHandler.Instance.OpenConnection();
            try
            {
                string id = UserSession.Instance.User.Employee.EmployeeId;
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL("select current_cet from public.employee where id_employee = " + id + ";");
                if (result != null)
                {
                    while (result.Read())
                    {
                        CETCurrentNumber = int.Parse(result[0].ToString());
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

                if (int.Parse(NbDays) > CETCurrentNumber)
                {
                    IsCorrect = "Non";
                    MessageBox.Show("Cette demande de congés concerne " + NbDays + " jours ouvrés, or vous n'avez que " + CETCurrentNumber + " jours dans votre CET. \nVeuillez ajouter des jours dans votre CET ou réduire la durée des congés.", "Erreur");
                }
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

                // Le status est défaut à 2 (En attente de validation) pour le moment
                // Il faudrait peut être le mettre à 1 (Créé) puis que l'utilisateur le confirme pour qu'il passe au status 2
                // TODO : Le type de congé est en dur pour le moment (08 = CP) mais il faudrait peut être le changer.
                int status = 2;
                try
                {
                    // Todo : améliorer gestion input utilisateur
                    Comment = Comment.Replace("'", "''");
                    string query = string.Format(@"INSERT INTO public.dayoff(id_employee, creation_date, status, id_day_off_type,
                                                       start_date, end_date, nb_days, employee_commentary)
                                                       VALUES({0}, date '{1}', {2}, {3}, date '{4}', date '{5}', {6}, '{7}');",
                                                               UserSession.Instance.User.Employee.EmployeeId, DateTime.Today.Date.ToShortDateString(), status, "08", StartDate, EndDate, NbDays, Comment);

                    DbHandler.Instance.ExecSQL(query);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erreur : " + e.Message);
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
