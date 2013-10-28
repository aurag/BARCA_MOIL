using System.Windows.Input;
using Barcelone___OGTS.Common;
using System;
using System.Collections.Generic;

namespace Barcelone___OGTS.ViewModel
{
    public class AddInCETViewModel : BaseViewModel
    {
        #region Commands
        public ICommand ClickBack { get; set; }
        #endregion

        #region Properties
        private string _label;
        private List<String> _leaveTypes = new List<String>();
        #endregion

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

        public AddInCETViewModel()
        {
            ClickBack = new Command(param => Back(), param => true);
            Label = Switcher.ApplicationState["label"] as string;
            _leaveTypes.Add("Congés principaux");
            _leaveTypes.Add("Congés 2");
            _leaveTypes.Add("Congés 3");
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
