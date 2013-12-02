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
    public class RHOperationsViewModel : BaseViewModel
    {
        #region Commands
        public ICommand ClickBack { get; set; }
        public ICommand HandleCheckBox { get; set; }
        #endregion

        #region Properties
        private ICollectionView _daysForValidation;
        public ICollectionView DaysForValidation 
        {
            get
            {
                return _daysForValidation;
            }
            private set
            {
                _daysForValidation = value;
                OnPropertyChanged("DaysForValidation");
            }
        }

        private ICollectionView _daysForValidationForecast;
        public ICollectionView DaysForValidationForecast
        {
            get
            {
                return _daysForValidationForecast;
            }
            private set
            {
                _daysForValidationForecast = value;
                OnPropertyChanged("DaysForValidationForecast");
            }
        }
        #endregion

        private string _employeeManaged;

        public RHOperationsViewModel()
        {
            ClickBack = new Command(param => Back(), param => true);
            HandleCheckBox = new Command(param => FirstHandleCheckBox(), param => true);
            GetEmployeesManaged();
            CreateDaysForValidationList();
            CreateDaysForValidationForecastList();
        }

        private void GetEmployeesManaged()
        {
            DbHandler.Instance.OpenConnection();
            try
            {

                NpgsqlDataReader result = DbHandler.Instance.ExecSQL("select id_employee from public.employee WHERE id_employee_rh = " + UserSession.Instance.User.Employee.EmployeeId + ";");
                if (result != null)
                {
                    Boolean first = true;
                    while (result.Read())
                    {
                        if (first)
                        {
                            _employeeManaged = result[0].ToString();
                            first = false;
                        }
                        else
                            _employeeManaged += " OR id_employee = " + result[0].ToString();
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

        #region Commands Methods
        #endregion

        #region CanExecute Methods
        private void Back()
        {
            Switcher.Switch(new HomeView());
        }

        private void CreateDaysForValidationForecastList()
        {
            DbHandler.Instance.OpenConnection();
            List<DayOff> _daysOff = null;
            try
            {
                _employeeManaged = _employeeManaged.Replace("id_employee", "public.dayoffforecast.id_employee");
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL(string.Format(@"select submission_date, firstname, lastname, start_date, end_date, nb_days, employee_commentary                                                                  employee_commentary
                                                                   from public.dayoffforecast INNER JOIN public.employee ON (public.dayoffforecast.id_employee = public.employee.id_employee)
                                                                   WHERE public.dayoffforecast.id_employee={0};", _employeeManaged));
                if (result != null)
                {
                    _daysOff = new List<DayOff>();
                    while (result.Read())
                    {
                        DayOff dayOff = new DayOff()
                        {
                            SubmissionDate = (result[0].ToString() == "" ? "" : result[0].ToString().Substring(0, 10)),
                            Name = result[1].ToString() + " " + result[2].ToString(),
                            StartDate = result[3].ToString().Substring(0, 10),
                            EndDate = result[4].ToString().Substring(0, 10),
                            NbDays = result[5].ToString(),
                            CommentSal = result[6].ToString()
                        };

                        _daysOff.Add(dayOff);
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
            DaysForValidationForecast = CollectionViewSource.GetDefaultView(_daysOff);
        }

        private void CreateDaysForValidationList()
        {
            DbHandler.Instance.OpenConnection();
            _employeeManaged = _employeeManaged.Replace("id_employee", "public.dayoff.id_employee");
            NpgsqlDataReader result = DbHandler.Instance.ExecSQL(string.Format(@"select start_date, end_date, submission_date, type, firstname, lastname,
                                                                   employee_commentary, superior_commentary
                                                                   from public.dayofftype, public.dayoff INNER JOIN public.employee ON (public.dayoff.id_employee = public.employee.id_employee)
                                                                   WHERE public.dayoff.id_day_off_type = public.dayofftype.id_day_off_type
                                                                   AND public.dayoff.id_employee={0}
                                                                   AND public.dayoff.status = 2;", _employeeManaged));
            List<DayOff> _daysOff = null;
            if (result != null)
            {
                _daysOff = new List<DayOff>();
                while (result.Read())
                {
                    DayOff dayOff = new DayOff()
                    {
                        StartDate = result[0].ToString().Substring(0, 10),
                        EndDate = result[1].ToString().Substring(0, 10),
                        SubmissionDate = DateTime.Today.ToShortDateString(),
                        Type = result[3].ToString(),
                        Name = result[4].ToString() + " " + result[5].ToString(),
                        CommentSal = result[6].ToString(),
                        CommentRh = result[7].ToString()
                    };

                    _daysOff.Add(dayOff);
                }
            }

            DbHandler.Instance.CloseConnection();
            DaysForValidation = CollectionViewSource.GetDefaultView(_daysOff);
        }

        // Gestion des cases cochées pour le premier tableau
        private void FirstHandleCheckBox()
        {
            try
            {
                DbHandler.Instance.OpenConnection();
                for (int i = 0; i < ((List<DayOff>)DaysForValidation.SourceCollection).Count; i++)
                {
                    DayOff day = ((List<DayOff>)DaysForValidation.SourceCollection)[i];
                    if (day.IsSelectedOk)
                    {
                        DbHandler.Instance.ExecSQL(string.Format(@"UPDATE dayoff SET status = 5  
                                                               WHERE start_date = (date '{0}') and end_date = (date '{1}') and id_employee = {2}",
                                                                   day.StartDate, day.EndDate, UserSession.Instance.User.Employee.EmployeeId));
                        ((List<DayOff>)DaysForValidation.SourceCollection).Remove(day);
                        i--;
                    }
                    if (day.IsSelectedNok)
                    {
                        DbHandler.Instance.ExecSQL(string.Format(@"UPDATE dayoff SET status = 6  
                                                               WHERE start_date = (date '{0}') and end_date = (date '{1}') and id_employee = {2}",
                                                                   day.StartDate, day.EndDate, UserSession.Instance.User.Employee.EmployeeId));
                        ((List<DayOff>)DaysForValidation.SourceCollection).Remove(day);
                        i--;
                    }
                }
                DaysForValidation.Refresh();
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
        #endregion
    }
}
