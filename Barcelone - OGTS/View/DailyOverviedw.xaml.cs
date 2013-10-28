using Barcelone___OGTS.Common;
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

namespace Barcelone___OGTS.View
{
    /// <summary>
    /// Logique d'interaction pour DailyOverview.xaml
    /// </summary>
    public partial class DailyOverview : UserControl, ISwitchable
    {
        public DailyOverview()
        {
            InitializeComponent();
            DataContext = new DailyOverview();
        }

        #region ISWitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
