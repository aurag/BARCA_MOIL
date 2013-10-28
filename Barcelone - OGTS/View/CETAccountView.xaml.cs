using Barcelone___OGTS.Common;
using Barcelone___OGTS.ViewModel;
using System;
using System.Windows.Controls;

namespace Barcelone___OGTS.View
{
    /// <summary>
    /// Logique d'interaction pour CETAccountView.xaml
    /// </summary>
    public partial class CETAccountView : UserControl, ISwitchable
    {
        public CETAccountView()
        {
            InitializeComponent();
            DataContext = new CETAccountViewModel();
        }

        #region ISWitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
