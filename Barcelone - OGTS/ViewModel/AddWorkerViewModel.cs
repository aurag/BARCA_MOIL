using Barcelone___OGTS.Common;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Barcelone___OGTS.ViewModel
{
    public class AddWorkerViewModel : BaseViewModel
    {
        
        #region Commandes
        public ICommand BackCommand { get; set; }
        #endregion

        #region Properties
        private string _label;
        private List<string> _leaveFamily = new List<string>();
        private List<string> _leavePosition = new List<string>();

        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                this.OnPropertyChanged("Label");
            }
        }

        public List<string> LeaveFamily
        {
            get { return _leaveFamily; }
            set
            {
                _leaveFamily = value;
                this.OnPropertyChanged("LeaveFamily");
            }
        }

        public List<string> LeavePosition
        {
            get { return _leavePosition; }
            set
            {
                _leavePosition = value;
                this.OnPropertyChanged("LeavePosition");
            }
        }

        #endregion

        /// <summary>
        /// constructeur
        /// </summary>

        public AddWorkerViewModel()
        {
            BackCommand = new Command(param => Back(), param => true);
            Label = Switcher.ApplicationState["label"] as string;
            _leaveFamily.Add("Célibataire");
            _leaveFamily.Add("Marié(e)");
            _leaveFamily.Add("Divorcé(e)");
            _leaveFamily.Add("Veuf(ve)");

            _leavePosition.Add("IC1");
            _leavePosition.Add("IC2");
            _leavePosition.Add("IC3");
        }

        #region Commands Methods
        private void Back()
        {
            Switcher.SwitchBack();
        }
        #endregion

        #region CanExecute Methods
        #endregion
    }
}