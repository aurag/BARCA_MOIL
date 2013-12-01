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
        #endregion

        public RHOperationsViewModel()
        {
            ClickBack = new Command(param => Back(), param => true);
            HandleCheckBox = new Command(param => FirstHandleCheckBox(), param => true);
            CreateDaysForValidationList();
        }

        #region Commands Methods
        #endregion

        #region CanExecute Methods
        private void Back()
        {
            Switcher.Switch(new HomeView());
        }

        // Todo : Create the operations list
        private void CreateDaysForValidationList()
        {
            DbHandler.Instance.OpenConnection();

            NpgsqlDataReader result = DbHandler.Instance.ExecSQL(string.Format(@"select start_date, end_date, creation_date, type, title, status, 
                                                                   employee_commentary, superior_commentary, validation_date
                                                                   from public.dayoff, public.dayofftype
                                                                   WHERE public.dayoff.id_day_off_type = public.dayofftype.id_day_off_type
                                                                   AND public.dayoff.id_employee={0}
                                                                   AND public.dayoff.status = 2;", UserSession.Instance.User.Employee.EmployeeId));
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
                        CreationDate = DateTime.Today.ToShortDateString(),
                        Type = result[3].ToString(),
                        Title = result[4].ToString(),
                        Status = result[5].ToString(),
                        CommentSal = result[6].ToString(),
                        CommentRh = result[7].ToString(),
                        DateRh = result[8].ToString(),
                        IdEmployee = UserSession.Instance.User.Employee.EmployeeId
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
