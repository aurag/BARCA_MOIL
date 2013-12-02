using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Barcelone___OGTS.Model
{
    public class DayOff : INotifyPropertyChanged
    {
        #region Fields

        private string _startDate;
        private string _endDate;
        private string _idEmployee;
        private string _creationDate;
        private string _submissionDate;
        private string _cancelRequestDate;
        private string _cancelDate;
        private string _nbDays;
        private string _type; 
        private string _name; 
        private string _status;
        private string _commentSal;
        private string _commentRh;
        private string _dateRh;
        private Boolean _isSelected;
        private Boolean _isSelected2;
        private Boolean _isSelected3;
        private Boolean _isSelected4;

        #endregion

        #region Properties

        private Boolean _isSelectedOk;

        public Boolean IsSelectedOk
        {
            get { return _isSelectedOk; }
            set 
            {
                if (value && _isSelectedNok)
                    IsSelectedNok = false;
                _isSelectedOk = value;
                NotifyPropertyChanged("IsSelectedOk");
            }
        }
        private Boolean _isSelectedNok;

        public Boolean IsSelectedNok
        {
            get { return _isSelectedNok; }
            set 
            {
                if (value && _isSelectedOk)
                    IsSelectedOk = false; 
                _isSelectedNok = value; 
                NotifyPropertyChanged("IsSelectedNok");
            }
        }

        public string StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                NotifyPropertyChanged("StartDate");
            }
        }

        public string IdEmployee
        {
            get { return _idEmployee; }
            set
            {
                _idEmployee = value;
                NotifyPropertyChanged("IdEmployee");
            }
        }

        public string EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                NotifyPropertyChanged("EndDate");
            }
        }

        public string CreationDate
        {
            get { return _creationDate; }
            set
            {
                _creationDate = value;
                NotifyPropertyChanged("CreationDate");
            }
        }

        public string SubmissionDate
        {
            get { return _submissionDate; }
            set
            {
                _submissionDate = value;
                NotifyPropertyChanged("SubmissionDate");
            }
        }

        public string CancelRequestDate
        {
            get { return _cancelRequestDate; }
            set
            {
                _cancelRequestDate = value;
                NotifyPropertyChanged("CancelRequestDate");
            }
        }

        public string CancelDate
        {
            get { return _cancelDate; }
            set
            {
                _cancelDate = value;
                NotifyPropertyChanged("CancelDate");
            }
        }

        public string NbDays
        {
            get { return _nbDays; }
            set
            {
                _nbDays = value;
                NotifyPropertyChanged("NbDays");
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

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        // Status : 0 : Non défini
        // 1 : Créé, 2 : En attente de validation, 3 : Annulé, 4 : En demande d'annulation, 5 : Accepté, 6 : Refusé
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyPropertyChanged("Status");
            }
        }

        public string CommentSal
        {
            get { return _commentSal; }
            set
            {
                _commentSal = value;
                NotifyPropertyChanged("CommentSal");
            }
        }

        public string CommentRh
        {
            get { return _commentRh; }
            set
            {
                _commentRh = value;
                NotifyPropertyChanged("CommentRh");
            }
        }

        public string DateRh
        {
            get { return _dateRh; }
            set
            {
                _dateRh = value;
                NotifyPropertyChanged("DateRh");
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

        public Boolean IsSelected3
        {
            get { return _isSelected3; }
            set
            {
                _isSelected3 = value;
                NotifyPropertyChanged("IsSelected3");
            }

        }

        public Boolean IsSelected4
        {
            get { return _isSelected4; }
            set
            {
                _isSelected3 = value;
                NotifyPropertyChanged("IsSelected4");
            }

        }
        #endregion

        #region Constructor
        public DayOff()
        {
            _isSelected = false;
            _isSelected2 = false;
            _isSelected3 = false;
            _isSelected4 = false;
            _nbDays = "";
        }

        [Obsolete("You should use the empty constructor", true)]
        public DayOff(string startDate, string endDate, string creationDate, string submissionDate, string type, string label,
                      string status, string commentSal, string commentRh, string dateRh)
            : this()
        {
            _startDate = startDate;
            _endDate = endDate;
            _creationDate = creationDate;
            _type = type;
            _name = label;
            _status = status;
            _commentSal = commentSal;
            _commentRh = commentRh;
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
