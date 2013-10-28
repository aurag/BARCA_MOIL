using Barcelone___OGTS.Common;
using Barcelone___OGTS.View;
using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Controls;
using Npgsql;
using System.Windows;

namespace Barcelone___OGTS.ViewModel
{
    public class FirstViewModel : BaseViewModel
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
                NotifyPropertyChanged("Login");
            }
        }
        #endregion

        /// <summary>   
        /// constructeur
        /// </summary>
        public FirstViewModel()
        {
            ClickCommand = new Command(param => Authentification(param), param => true);
            AdminView = new Command(param => PushAdmin(), param => true);
            RHView = new Command(param => PushRH(), param => true);

            Login = "201301";
        }

        /// <summary>
        /// réponse à la commande click
        /// </summary>
        private void Authentification(object parameter)
        {
            PasswordBox passwordBox = parameter as PasswordBox;
            String password = passwordBox.Password;
            bool isCorrect = false;

            DbHandler.Instance.OpenConnection();
            NpgsqlDataReader result = DbHandler.Instance.ExecSQL("select login, password from public.userogts;");
            while (result.Read())
            {
                if ((Login.Equals(result[0])) && (password.Equals(result[1])))
                {
                    isCorrect = true;
                    break;
                }
            }

            if (isCorrect)
            {
                DbHandler.Instance.CloseConnection();
                Switcher.Switch(new SecondView());
            }
            else
            {
                DbHandler.Instance.CloseConnection();
                MessageBox.Show("Mot de passe ou nom d'utilisateur invalide", "Erreur d'authentification");
            }
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


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Helpers

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
