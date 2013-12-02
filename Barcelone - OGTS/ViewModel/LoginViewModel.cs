using Barcelone___OGTS.Common;
using Barcelone___OGTS.View;
using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Controls;
using Npgsql;
using System.Windows;
using Barcelone___OGTS.Model;

namespace Barcelone___OGTS.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        #region Commandes
        public ICommand ClickCommand { get; set; }
        public ICommand AdminView { get; set; }
        public ICommand RHView { get; set; }
        #endregion

        #region Properties

        private string _login;

        public string Login
        {
            get
            {
                return _login;
            }
            set
            {
                _login = value;
                OnPropertyChanged("Login");
            }
        }
        #endregion

        /// <summary>   
        /// constructeur
        /// </summary>
        public LoginViewModel()
        {
            ClickCommand = new Command(param => Authentification(param), param => true);
            AdminView = new Command(param => PushAdmin(), param => true);
            RHView = new Command(param => PushRH(), param => true);

            Login = "201301";
        }

        /// <summary>
        /// Authentification of the user
        /// </summary>
        private void Authentification(object parameter)
        {
            PasswordBox passwordBox = parameter as PasswordBox;
            string password = passwordBox.Password;
            bool isCorrect = false;

            if (!CheckLoginAndPassword(Login, password))
                return;
    
            DbHandler.Instance.OpenConnection();
            string query = string.Format("select login, password, firstname, lastname, last_connection_date, last_connection_time, public.userogts.id_user, public.employee.id_employee from public.userogts " +
              "INNER JOIN public.employee ON (userogts.id_user = employee.id_user) where login='{0}' AND password='{1}';", Login, password);

            NpgsqlDataReader result = DbHandler.Instance.ExecSQL(query);

            if (!result.HasRows)
            {
                DbHandler.Instance.CloseConnection();
                MessageBox.Show("Mot de passe ou nom d'utilisateur invalide", "Erreur d'authentification");
                return;
            }

            while (result.Read())
            {
                if ((Login.Equals(result[0])) && (password.Equals(result[1])))
                {
                    isCorrect = true;
                    UserSession session = UserSession.Instance;
                    User user = new User();
                    Employee employee = new Employee();
                    employee.Matricule = Login;
                    employee.Firstname = result[2] as string;
                    employee.Lastname = result[3] as string;
                    if (result[4].ToString() != "")
                        user.LastConnectionDate = result[4].ToString().Substring(0, 10);
                    else
                        user.LastConnectionDate = "Première connexion";
                    if (result[5].ToString() != "")
                        user.LastConnectionTime = result[5].ToString().Substring(11);
                    else
                        user.LastConnectionTime = " ";
                    int? userId = result[6] as int?;
                    user.UserId = userId.ToString();
                    int? employeeId = result[7] as int?;
                    employee.EmployeeId = employeeId.ToString();
                    user.Employee = employee;
                    session.User = user;

                    break;
                }
            }
            DbHandler.Instance.CloseConnection();
            DbHandler.Instance.OpenConnection();
            try
            {
                string EmployeeId = UserSession.Instance.User.Employee.EmployeeId;
                DbHandler.Instance.ExecSQL("UPDATE userogts SET last_connection_date = date '" + DateTime.Today.ToShortDateString() + "' where id_employee = " + EmployeeId + ";");
                DbHandler.Instance.ExecSQL("UPDATE userogts SET last_connection_time = time '" + string.Format("{0:HH:mm:ss tt}", DateTime.Now) + "' where id_employee = " + EmployeeId + ";");
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur : " + e.Message);
            }
            finally
            {
                DbHandler.Instance.CloseConnection();
            }
            UserSession.Instance.User.Employee.IsRH = DbHandler.Instance.checkIfRh( UserSession.Instance.User.Employee.EmployeeId);

            if (isCorrect)
            {
                if (GetStatus())
                    Switcher.Switch(new HomeView());
                else
                    MessageBox.Show("La convention collective n'a pas été validée. L'administrateur doit la valider pour permettre l'utilisation du logiciel OGTS", "Erreur d'administration");
            }
            else
                MessageBox.Show("Mot de passe ou nom d'utilisateur invalide", "Erreur d'authentification");
        }

        private Boolean GetStatus()
        {
            DbHandler.Instance.OpenConnection();

            try
            {

                NpgsqlDataReader result = DbHandler.Instance.ExecSQL("select first_operating_date from organizationchart;");
                if (result != null)
                {
                    while (result.Read())
                    {
                        if (result[0].ToString() != "")
                        {
                            return true;
                        }
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
            return false;
        }

        /// <summary>
        /// Returns true if both are valid, false else.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private Boolean CheckLoginAndPassword(string login, string password)
        {
            if (password.Contains("&") || password.Contains(";"))
            {
                MessageBox.Show("Mauvais format de mot de passe", "Erreur d'authentification");
                return false;
            }

            if (login.Length != 6)
            {
                MessageBox.Show("Mauvais format de login : vous devez utiliser votre matricule", "Erreur d'authentification");
                return false;
            }

            return true;
        }

        private void PushAdmin()
        {
            //Switcher.Switch(new AddWorker());
            //Switcher.Switch(new RHHomeView()); RH
            Switcher.Switch(new AdminHomeView());
        }
        private void PushRH()
        {
            //Switcher.Switch(new AddWorker());
            Switcher.Switch(new RHHomeView());
        }

    }
}
