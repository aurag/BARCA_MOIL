using Barcelone___OGTS.Common;
using Barcelone___OGTS.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace Barcelone___OGTS.ViewModel
{
    public class CollectiveAgreementViewModel : BaseViewModel
    {
        #region Commands
        public ICommand BackCommand { get; set; }
        #endregion

        #region Properties
        public ICollectionView cas { get; private set; }
        #endregion

        public CollectiveAgreementViewModel()
        {
            CreateCasData();
            BackCommand = new Command(param => Back(), param => true);
        }

        #region Commands Methods
        private void Back()
        {
            Switcher.SwitchBack();
        }
        #endregion

        #region CanExecute Methods

        private void CreateCasData()
        {
            var _cas = new List<CollectiveAgreement>
                {
                    new CollectiveAgreement("10/02/13"),
                    new CollectiveAgreement("02/02/13"),
                };

            cas = CollectionViewSource.GetDefaultView(_cas);
        }
        #endregion
    }
}