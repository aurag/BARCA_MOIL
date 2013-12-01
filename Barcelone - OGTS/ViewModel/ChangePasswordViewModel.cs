using Barcelone___OGTS.Common;
using Barcelone___OGTS.View;
using Npgsql;
using System;
using System.Security;
using System.Windows;
using System.Windows.Input;

namespace Barcelone___OGTS.ViewModel
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        #region Commands
        public ICommand BackCommand { get; set; }
        #endregion

        #region Properties

        private static string _oldPassword;

        public static string OldPassword
        {
            get { return _oldPassword; }
            set { _oldPassword = value; }
        }
        private static string _newPassword1;

        public static string NewPassword1
        {
            get { return _newPassword1; }
            set { _newPassword1 = value; }
        }
        private static string _newPassword2;

        public static string NewPassword2
        {
            get { return _newPassword2; }
            set { _newPassword2 = value; }
        }

        #endregion

        public ChangePasswordViewModel()
        {
            BackCommand = new Command(param => Back(), param => true);
        }

        #region Commands Methods
        private void Back()
        {
            Switcher.SwitchBack();
        }

        public static void ChangePassword()
        {
            if (string.IsNullOrEmpty(OldPassword) || string.IsNullOrEmpty(NewPassword1) || string.IsNullOrEmpty(NewPassword2))
            {
                MessageBox.Show("Merci de remplir tous les champs");
                return;
            }

            if (NewPassword1 != NewPassword2)
            {
                MessageBox.Show("Les deux champs 'nouveau mot de passe' ne sont pas les mêmes", "Erreur");
                return;
            }

            DbHandler.Instance.OpenConnection();
            try
            {
                string login = UserSession.Instance.User.Employee.Matricule;
                string query = string.Format("select login, password from public.userogts where login='{0}' AND password='{1}';", login, OldPassword.ToString());

                NpgsqlDataReader result = DbHandler.Instance.ExecSQL(query);

                if (!result.HasRows)
                {
                    MessageBox.Show("Mot de passe invalide", "Erreur d'authentification");
                    return;
                }

                result.Close();
                DbHandler.Instance.ExecSQL("update public.userogts set password = '" + NewPassword2 + "' where login = '" + login + "';");
                MessageBox.Show("Mot de passe changé avec succès !", "Succès");
                DbHandler.Instance.CloseConnection();
                Switcher.Switch(new HomeView());
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

        #region CanExecute Methods
        #endregion
    }
}
