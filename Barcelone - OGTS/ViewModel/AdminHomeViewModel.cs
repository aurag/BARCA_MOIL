using Barcelone___OGTS.Common;
using Barcelone___OGTS.Model;
using Barcelone___OGTS.View;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace Barcelone___OGTS.ViewModel
{
    public class AdminHomeViewModel : BaseViewModel
    {
        #region Commands
        public ICommand BackCommand { get; set; }
        public ICommand GoToConventionAgreementView { get; set; }
        public ICommand GoToOrganigramView { get; set; }
        public ICommand GoToPersonelView { get; set; }
        public ICommand GoToAddEmployeeView { get; set; }
        
        #endregion

        #region Properties
        public ICollectionView cas { get; private set; }
        public ICollectionView Organigrams { get; private set; }
        #endregion

        public AdminHomeViewModel()
        {
            BackCommand = new Command(param => Back(), param => true);
            GoToConventionAgreementView = new Command(param => PushConventionAgreementView(), param => true);
            GoToOrganigramView = new Command(param => PushOrganigramView(), param => true);
            GoToPersonelView = new Command(param => PushPersonelView(), param => true);
            GoToAddEmployeeView = new Command(param => PushAddWorkerView(), param => true);
            CreateCasData();
        }

        #region Commands Methods
        private void Back()
        {
            Switcher.SwitchBack();
        }


        private void PushConventionAgreementView()
        {
            Switcher.Switch(new CollectiveAgreementView());
        }

        private void PushOrganigramView()
        {
            Switcher.Switch(new OrganigramView());
        }

        private void PushPersonelView()
        {
            Switcher.Switch(new CollectiveAgreementView());
        }
        private void PushAddWorkerView()
        {
            Switcher.Switch(new AddWorker());
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

            var _orga = new List<Organigram>
                {
                    new Organigram("10/02/13"),
                    new Organigram("02/02/13"),
                    new Organigram("20/01/13"),
                    new Organigram("04/01/13")
                };

            Organigrams = CollectionViewSource.GetDefaultView(_orga);
        }

        #endregion
    }
}
