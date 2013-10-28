using Barcelone___OGTS.Common;
using Barcelone___OGTS.Model;
using Barcelone___OGTS.View;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace Barcelone___OGTS.ViewModel
{
    public class RHOperationsViewModel : BaseViewModel
    {
        #region Commands
        public ICommand ClickBack { get; set; }
        #endregion

        #region Properties
        public ICollectionView asking { get; private set; }
        #endregion

        public RHOperationsViewModel()
        {
            ClickBack = new Command(param => Back(), param => true);
            CreateAskingData();
        }

        #region Commands Methods
        #endregion

        #region CanExecute Methods
        private void Back()
        {
            Switcher.SwitchBack();
        }

        // Todo : Create the operations list
        private void CreateAskingData()
        {
            /*
            var _asking = new List<DayOff>
                {
                    new DayOff("02/08/13", "21/08/13", "23/03/13", "01", "Congé principal", "", "Mariage de mon fils", "", ""),
                    new DayOff("15/12/13", "08/01/14", "23/03/13", "01", "Congé principal", "", "", "", ""),
                };

            asking = CollectionViewSource.GetDefaultView(_asking);
             */
        }
        #endregion
    }
}
