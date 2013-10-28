using Barcelone___OGTS.Common;
using System;
using System.Windows.Controls;

namespace Barcelone___OGTS.View
{
    /// <summary>
    /// Logique d'interaction pour Switchable_UserControl1.xaml
    /// </summary>
    public partial class AddInCET: UserControl, ISwitchable
    {
        public AddInCET()
        {
            InitializeComponent();
        }

        #region ISWitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }
        #endregion

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }
    }
}
