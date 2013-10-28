using Barcelone___OGTS.Common;
using System.Windows.Input;

namespace Barcelone___OGTS.ViewModel
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        #region Commands
        public ICommand BackCommand { get; set; }
        #endregion

        #region Properties
        #endregion

        public ChangePasswordViewModel()
        {
            BackCommand = new Command(param => Back(), param => true);
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
