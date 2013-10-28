using Barcelone___OGTS.Common;
using System;
using System.Windows.Controls;


namespace Barcelone___OGTS.View
{
    /// <summary>
    /// Logique d'interaction pour Switchable_UserControl1.xaml
    /// </summary>
    public partial class LeaveRequestView : UserControl, ISwitchable
    {
        public LeaveRequestView()
        {
            InitializeComponent();
        }

        #region ISWitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
