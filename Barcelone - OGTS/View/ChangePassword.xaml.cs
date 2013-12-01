using Barcelone___OGTS.Common;
using Barcelone___OGTS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Barcelone___OGTS.View
{
    /// <summary>
    /// Logique d'interaction pour Switchable_UserControl1.xaml
    /// </summary>
    public partial class ChangePassword : UserControl, ISwitchable
    {
        public ChangePassword()
        {
            InitializeComponent();
        }

        #region ISWitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordViewModel.OldPassword = PasswordBoxOld.Password;
            ChangePasswordViewModel.NewPassword1 = PasswordBox1.Password;
            ChangePasswordViewModel.NewPassword2 = PasswordBox2.Password;
            ChangePasswordViewModel.ChangePassword();
        }


    }
}
