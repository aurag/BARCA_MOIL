using Barcelone___OGTS.Common;
using Barcelone___OGTS.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace Barcelone___OGTS.ViewModel
{
    public class OrganigramViewModel : BaseViewModel
    {
        #region Commands
        public ICommand BackCommand { get; set; }
        #endregion

        #region Properties
        public ICollectionView orgas { get; private set; }
        #endregion

        public OrganigramViewModel()
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
            var _orgas = new List<Organigram>
                {
                    new Organigram("10/02/13"),
                    new Organigram("02/02/13"),
                    new Organigram("20/01/13"),
                    new Organigram("04/01/13")
                };

            orgas = CollectionViewSource.GetDefaultView(_orgas);
        }
        #endregion
    }
}