using Barcelone___OGTS.Common;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Barcelone___OGTS.Model;
using System.Windows.Data;
using System.ComponentModel;
using Npgsql;

namespace Barcelone___OGTS.ViewModel
{
    class DailyOverviewViewModel : BaseViewModel
    {
         #region Commandes
        public ICommand BackCommand { get; set; }
        #endregion

        #region Properties
        private string _label;
        private List<String> _leaveTypes = new List<String>();

        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                this.OnPropertyChanged("Label");
            }
        }

        public List<String> LeaveTypes
        {
            get { return _leaveTypes; }
            set
            {
                _leaveTypes = value;
                this.OnPropertyChanged("LeaveTypes");
            }
        }

        public ICollectionView leaveRequests { get; private set; }

        #endregion

        /// <summary>
        /// constructeur
        /// </summary>
        public DailyOverviewViewModel()
        {
            BackCommand = new Command(param => Back(), param => true);
            Label = Switcher.ApplicationState["label"] as string;
            _leaveTypes.Add("Congés principaux");
            _leaveTypes.Add("Congés 2");
            _leaveTypes.Add("Congés 3");

            // Creation de la liste de demandes de congés pour les tableaux.
            CreateLeaveRequestList();
        }

        /// <summary>
        /// réponse à la commande back
        /// </summary>
        private void Back()
        {
            Switcher.SwitchBack();
        }

        /// <summary>
        /// Todo : Creates the liste of days off request waiting for validation
        /// </summary>
        private void CreateLeaveRequestList()
        {
            /*
            var _leaveRequests = new List<DayOff>
                {
                    new DayOff("02/04/13", "11/04/13", "01", "23/03/13", "Congé principal", "En attente", "", "", "12/03/13"),
                    new DayOff("12/04/13", "15/04/13", "01", "23/03/13", "Congé principal", "En attente", "", "", "15/03/13"),
                    new DayOff("24/04/13", "28/04/13", "01", "23/03/13", "Congé principal", "En attente", "", "", "19/03/13")
                };

            leaveRequests = CollectionViewSource.GetDefaultView(_leaveRequests);
             */
        }
    }
}
