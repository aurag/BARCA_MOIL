using System.Windows.Input;
using Barcelone___OGTS.Common;
using System;
using System.Collections.Generic;
using System.Windows;
using Npgsql;
using Barcelone___OGTS.View;

namespace Barcelone___OGTS.ViewModel
{
    public class AddInCETViewModel : BaseViewModel
    {
        #region Commands
        public ICommand ClickBack { get; set; }
        public ICommand ClickAdd { get; set; }
        #endregion

        #region Properties
        private string _label;
        private List<String> _leaveTypes = new List<String>();
        private String _daysToAdd;

        public String DaysToAdd
        {
            get { return _daysToAdd; }
            set { _daysToAdd = value; }
        }
        #endregion

        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                this.OnPropertyChanged("Label");
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

        public AddInCETViewModel()
        {
            ClickBack = new Command(param => Back(), param => true);
            ClickAdd = new Command(param => Add(), param => true);
            Label = Switcher.ApplicationState["label"] as string;
            _leaveTypes.Add("Congés principaux");
            _leaveTypes.Add("Congés 2");
            _leaveTypes.Add("Congés 3");
        }

        #region Commands Methods
        private void Back()
        {
            Switcher.SwitchBack();
        }

        // Gestion de l'ajout dans le CET
        private void Add()
        {
            int number;
            if (int.TryParse(DaysToAdd, out number))
            {
                if (number > 10)
                {
                    MessageBox.Show("Votre CET peut contenir au maximum 10 jours");
                    return;
                }

                int lastNumber = getCurrentCET();

                DbHandler.Instance.OpenConnection();
                try 
                {
                    String employeeId = UserSession.Instance.User.Employee.EmployeeId;
                    // Attention : le type de congé est à 8 par défaut actuellement
                    String query = "insert into cethistory (id_employee, action_date, action_type, nb_before, nb_after, id_day_off_type) " +
                                   "VALUES (" + employeeId + ", date '" + DateTime.Today.Date.ToShortDateString() + "', 'Ajout', " + lastNumber +
                                   ", " + (lastNumber - number).ToString() + ", 8);";
                    NpgsqlDataReader result = DbHandler.Instance.ExecSQL(query);
                    Switcher.Switch(new CETAccountView());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erreur création jour CET : " + e.Message);
                }
                finally
                {
                    DbHandler.Instance.CloseConnection();
                }
            }
            else
            {
                MessageBox.Show("Merci d'entrer un nombre valide");
                return;
            }
        }
        #endregion

        private int getCurrentCET()
        {
            int res = -1;
            DbHandler.Instance.OpenConnection();
            try 
            {
                String employeeId = UserSession.Instance.User.Employee.EmployeeId;
                String query = "select current_cet from public.employee where public.employee.id_employee = " + employeeId + ";"; 
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL(query);
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
                Console.WriteLine("Erreur récupération CET actuel : " + e.Message);
            }
            finally
            {
                DbHandler.Instance.CloseConnection();
            }

            return res;
        }

        #region CanExecute Methods
        #endregion
    }
}
