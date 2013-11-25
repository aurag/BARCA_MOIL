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

        private String _login;

        public String Login
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
            String password = passwordBox.Password;
            bool isCorrect = false;

            if (!CheckLoginAndPassword(Login, password))
                return;
    
            DbHandler.Instance.OpenConnection();
            String query = String.Format("select login, password, firstname, lastname, public.userogts.id_user, public.employee.id_employee from public.userogts " +
              "INNER JOIN public.employee ON (userogts.id_user = employee.id_user) where login='{0}' AND password='{1}';", Login, password);

            NpgsqlDataReader result = DbHandler.Instance.ExecSQL(query);

            if (result == null)
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
                    employee.Firstname = result[2] as String;
                    employee.Lastname = result[3] as String;
                    int? userId = result[4] as int?;
                    user.UserId = userId.ToString();
                    int? employeeId = result[5] as int?;
                    employee.EmployeeId = employeeId.ToString();
                    user.Employee = employee;
                    session.User = user;

                    break;
                }
            }
            DbHandler.Instance.CloseConnection();
            UserSession.Instance.User.Employee.IsRH = DbHandler.Instance.checkIfRh( UserSession.Instance.User.Employee.EmployeeId);

            if (isCorrect)
                Switcher.Switch(new HomeView());
            else
                MessageBox.Show("Mot de passe ou nom d'utilisateur invalide", "Erreur d'authentification");
        }

        /// <summary>
        /// Returns true if both are valid, false else.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private Boolean CheckLoginAndPassword(String login, String password)
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
