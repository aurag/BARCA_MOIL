using Barcelone___OGTS.Common;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Barcelone___OGTS.Model;
using System.Windows.Data;
using System.ComponentModel;
using Barcelone___OGTS.View;

namespace Barcelone___OGTS.ViewModel
{
    class CETAccountViewModel : BaseViewModel
    {
         #region Commandes
        public ICommand BackCommand { get; set; }
        public ICommand GoToAddInCET { get; set; }
        public ICommand GoToLeaveRequest { get; set; }
        #endregion

        #region Properties

        public ICollectionView CETOperations { get; private set; }

        #endregion

        /// <summary>
        /// constructeur
        /// </summary>
        public CETAccountViewModel()
        {
            BackCommand = new Command(param => Back(), param => true);
            GoToAddInCET = new Command(param => PushAddInCET(), param => true);
            GoToLeaveRequest = new Command(param => PushLeaveRequest(), param => true);
            // Creation de la liste des opérations sur le CET.
            CreateCETOPList();
            
        }

        /// <summary>
        /// réponse à la commande back
        /// </summary>
        private void Back()
        {
            Switcher.SwitchBack();
        }

        private void PushAddInCET()
        {
            Switcher.Switch(new AddInCET());
        }

        private void PushLeaveRequest()
        {
            Switcher.Switch(new LeaveRequestView());
        }

        /// <summary>
        /// Créé la liste des congés en attente
        /// </summary>
        private void CreateCETOPList()
        {
            var _CETOP = new List<CETOperation>
                {
                    new CETOperation("10/02/13", "Ajout", "03", "Congés supplémentaires", "2", "6"),
                    new CETOperation("02/02/13", "Retrait", "03", "Congés supplémentaires", "4", "2"),
                    new CETOperation("20/01/13", "Ajout", "02", "Congés d'ancienneté", "2", "4"),
                    new CETOperation("04/01/13", "Ajout", "02", "Congés d'ancienneté", "0", "2")
                };

            CETOperations = CollectionViewSource.GetDefaultView(_CETOP);
        }
    }
}
