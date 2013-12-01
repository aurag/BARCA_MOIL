using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Barcelone___OGTS.Model
{
    class Organigram : INotifyPropertyChanged
    {
        #region Constructor
        public Organigram(string s)
        {
            _sigle = s;
            _dateCreation = "19/02/2013";
            _dateMEP = "22/02/2013";
            _status = "En cours de constructions";
            _version = "1.0";
            _designation = "Nouvelle version";
            _RHMatricule = "MA904308";
            _SalMatricule = "MA9034508";
            _isSelected = false;
            _isSelected2 = false;
        }
        #endregion

        #region Getters and Setters
        private string _sigle;
        private string _dateCreation;
        private string _dateMEP;
        private string _status;
        private string _version;
        private string _designation;
        private string _RHMatricule;
        private string _SalMatricule;

        private Boolean _isSelected;
        private Boolean _isSelected2;

        public string Sigle
        {
            get { return _sigle; }
            set
            {
                _sigle = value;
                NotifyPropertyChanged("Sigle");
            }
        }

        public string DateMEP
        {
            get { return _dateMEP; }
            set
            {
                _dateMEP = value;
                NotifyPropertyChanged("DateMEP");
            }
        }

        public string DateCreation
        {
            get { return _dateCreation; }
            set
            {
                _dateCreation = value;
                NotifyPropertyChanged("DateCreation");
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyPropertyChanged("Status");
            }
        }

        public string Version
        {
            get { return _version; }
            set
            {
                _version = value;
                NotifyPropertyChanged("Version");
            }
        }

        public string Designation
        {
            get { return _designation; }
            set
            {
                _designation = value;
                NotifyPropertyChanged("Designation");
            }
        }

        public string RHMatricule
        {
            get { return _RHMatricule; }
            set
            {
                _RHMatricule = value;
                NotifyPropertyChanged("RHMatricule");
            }
        }

        public string SalMatricule
        {
            get { return _SalMatricule; }
            set
            {
                _SalMatricule = value;
                NotifyPropertyChanged("SalMatricule");
            }
        }

        public Boolean IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }
        public Boolean IsSelected2
        {
            get { return _isSelected2; }
            set
            {
                _isSelected2 = value;
                NotifyPropertyChanged("IsSelected2");
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Helpers

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
