using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Barcelone___OGTS.Model
{
    class CollectiveAgreement : INotifyPropertyChanged
    {
        #region Constructor
        public CollectiveAgreement(String date)
        {
            _addedDate = date;
            _lastModifiefDate = "15/02/2013";
            _type = "1";
            _label = "Congé principal";
            _isSelected = false;
            _isSelected2 = false;
        }

        #endregion

        #region Getters and Setters
        private String _addedDate;
        private String _lastModifiefDate;
        private String _type;
        private String _label;

        private Boolean _isSelected;
        private Boolean _isSelected2;

        public string AddedDate
        {
            get { return _addedDate; }
            set
            {
                _addedDate = value;
                NotifyPropertyChanged("AddedDate");
            }
        }

        public string LastModifiefDate
        {
            get { return _lastModifiefDate; }
            set
            {
                _lastModifiefDate = value;
                NotifyPropertyChanged("LastModifiefDate");
            }
        }

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                NotifyPropertyChanged("Type");
            }
        }

        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                NotifyPropertyChanged("Label");
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
