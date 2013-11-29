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
        private String _selectedLeaveType;

        private int _cETCurrentNumber;

        public int CETCurrentNumber
        {
            get { return _cETCurrentNumber; }
            set { _cETCurrentNumber = value; OnPropertyChanged("CETCurrentNumber"); }
        }

        private int _daysEligible;

        public int DaysEligible
        {
            get 
            {

                return _daysEligible;
            }
            set 
            { 
                _daysEligible = value;
                OnPropertyChanged("DaysEligible");
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
                    DaysEligible = getDaysEligible();
                    OnPropertyChanged("SelectedLeaveType");
                }
            }
        }
        
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

            // Création de la liste des types de congés
            DbHandler.Instance.OpenConnection();
            try
            {
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL("select title from public.dayofftype;");
                if (result != null)
                {
                    while (result.Read())
                    {
                        String tmp = result[0].ToString();
                        _leaveTypes.Add(tmp);
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

            // Mise à jour du solde du CET 
            DbHandler.Instance.OpenConnection();
            try
            {
                String id = UserSession.Instance.User.Employee.EmployeeId;
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

            try
            {
                SelectedLeaveType = LeaveTypes[0];
            }
            catch (Exception e)
            {
                Console.WriteLine("Liste des types de congés vide : " + e.Message);
            }
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

                if (number > DaysEligible)
                {
                    MessageBox.Show("Vous n'avez qu'au maximum " + DaysEligible + " jours disponibles pour ce type de congé");
                    return;
                }

                createCETHistory(number);
                Switcher.Switch(new CETAccountView());
            }
            else
            {
                MessageBox.Show("Merci d'entrer un nombre valide");
                return;
            }
        }



        // Création de l'entrée dans l'historique du CET
        private void createCETHistory(int number)
        {
            int lastNumber = getCurrentCET();
            String leaveTypeId = getLeaveTypeId();
            Boolean success = false;

            DbHandler.Instance.OpenConnection();
            try
            {
                String employeeId = UserSession.Instance.User.Employee.EmployeeId;
                if (leaveTypeId.Equals(""))
                    MessageBox.Show("Erreur lors de la récupération du type de congé");
                else
                {
                    String query = "insert into cethistory (id_employee, action_date, action_type, nb_before, nb_after, id_day_off_type) " +
                                   "VALUES (" + employeeId + ", date '" + DateTime.Today.Date.ToShortDateString() + "', 'Ajout', " + lastNumber +
                                   ", " + (lastNumber + number).ToString() + ", " + leaveTypeId + ");";
                    NpgsqlDataReader result = DbHandler.Instance.ExecSQL(query);
                    success = true;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur création jour CET : " + e.Message);
            }
            finally
            {
                DbHandler.Instance.CloseConnection();
            }

            if (success)
                updateDaysOff(number);
        }

        // Mise à jour des jours de congés disponibles pour le salariés et du solde du CET
        private void updateDaysOff(int numberDays)
        {
            String employeeId = UserSession.Instance.User.Employee.EmployeeId;
            String dayType = getLeaveTypeNumber();
            int solde = getDaysEligible();


            DbHandler.Instance.OpenConnection();
            try
            {
                int newNumber = solde - numberDays;

                // Todo : calculer le nouveau solde pour l'employé + mettre à jour le solde CET
                String queryUpdateDays = "update public.employee set days_type_" + dayType + " = " + newNumber + " where id_employee = " + employeeId + ";";
                DbHandler.Instance.ExecSQL(queryUpdateDays);
                String queryUpdateCET = "update public.employee set current_cet = " + (CETCurrentNumber + numberDays).ToString() + " where id_employee = " + employeeId + ";";
                DbHandler.Instance.ExecSQL(queryUpdateCET);
             }
            catch (Exception e)
            {
                Console.WriteLine("Erreur mise à jour solde après CET : " + e.Message);
            }
            finally
            {
                DbHandler.Instance.CloseConnection();
            }
        }

        #endregion

        private String getLeaveTypeId()
        {
            int index = LeaveTypes.IndexOf(SelectedLeaveType);
            String selectedLeaveTypeId = "";
            DbHandler.Instance.OpenConnection();
            try
            {
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL(String.Format("SELECT id_day_off_type FROM public.dayofftype;"));
                int i = 0;
                if (result != null)
                {
                    while (result.Read())
                    {
                        if (i == index)
                        {
                            selectedLeaveTypeId = result[0].ToString();
                            break;
                        }
                        i++;
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

            if (selectedLeaveTypeId.Equals(""))
                Console.WriteLine("Error : the type of day off was not found");

            return selectedLeaveTypeId;
        }

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

        private int getDaysEligible()
        {
            String dayTypeNumber = "";
            int res = 0;

            dayTypeNumber = getLeaveTypeNumber();

            String employeeId = UserSession.Instance.User.Employee.EmployeeId;

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

            return res;
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


        #region CanExecute Methods
        #endregion
    }
}
