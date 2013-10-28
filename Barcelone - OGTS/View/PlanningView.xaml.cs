using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Barcelone___OGTS.Common;
using Barcelone___OGTS.ViewModel;

namespace Barcelone___OGTS.View
{
    /// <summary>
    /// Logique d'interaction pour RequestAndProjectionView.xaml
    /// </summary>
    public partial class PlanningView : UserControl, ISwitchable
    {
        public PlanningView()
        {
            InitializeComponent();
            DataContext = new PlanningViewModel();
        }

        #region ISWitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
