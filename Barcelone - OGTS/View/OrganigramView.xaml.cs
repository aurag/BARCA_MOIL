using Barcelone___OGTS.Common;
using Barcelone___OGTS.ViewModel;
using System;
using System.Windows.Controls;

namespace Barcelone___OGTS.View
{
    /// <summary>
    /// Logique d'interaction pour Switchable_UserControl1.xaml
    /// </summary>
    public partial class OrganigramView : UserControl, ISwitchable
    {
        public OrganigramView()
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
