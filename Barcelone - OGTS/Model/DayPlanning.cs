using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;

namespace Barcelone___OGTS.Model
{
    class DayPlanning : INotifyPropertyChanged
    {
        #region Constructor
        public DayPlanning(Boolean IsAWorkDay, string LeaveLabelJanuary, string LeaveLabelFebruary, string LeaveLabelMarch, 
                           string LeaveLabelApril, string LeaveLabelMai, string LeaveLabelJune, string LeaveLabelJuly, 
                           string LeaveLabelAugust, string LeaveLabelSeptember,  string LeaveLabelOctober,  string LeaveLabelNovember,
                           string LeaveLabelDecember, int NbDay)
        {
            _isAWorkDay = IsAWorkDay;
            _leaveLabel1 = LeaveLabelJanuary;
            _leaveLabel2 = LeaveLabelFebruary;
            _leaveLabel3 = LeaveLabelMarch;
            _leaveLabel4 = LeaveLabelApril;
            _leaveLabel5 = LeaveLabelMai;
            _leaveLabel6 = LeaveLabelJune;
            _leaveLabel7 = LeaveLabelJuly;
            _leaveLabel8 = LeaveLabelAugust;
            _leaveLabel9 = LeaveLabelSeptember;
            _leaveLabel10 = LeaveLabelOctober;
            _leaveLabel11 = LeaveLabelNovember;
            _leaveLabel12 = LeaveLabelDecember;

            _nbDay = NbDay;
        }
        #endregion

        #region Getters and Setters

        private Boolean _isAWorkDay;
        private string _leaveLabel1;
        private string _leaveLabel2;
        private string _leaveLabel3;
        private string _leaveLabel4;
        private string _leaveLabel5;
        private string _leaveLabel6;
        private string _leaveLabel7;
        private string _leaveLabel8;
        private string _leaveLabel9;
        private string _leaveLabel10;
        private string _leaveLabel11;
        private string _leaveLabel12;
        private int _nbDay;

        public Boolean IsAWorkDay
        {
            get { return _isAWorkDay; }
            set
            {
                _isAWorkDay = value;
                NotifyPropertyChanged("IsAWorkDay");
            }
        }

        public string LeaveLabel1
        {
            get { return _leaveLabel1; }
            set
            {
                _leaveLabel1 = value;
                NotifyPropertyChanged("LeaveLabel1");
            }
        }

        public string LeaveLabel2
        {
            get { return _leaveLabel2; }
            set
            {
                _leaveLabel2 = value;
                NotifyPropertyChanged("LeaveLabel2");
            }
        }

        public string LeaveLabel3
        {
            get { return _leaveLabel3; }
            set
            {
                _leaveLabel3 = value;
                NotifyPropertyChanged("LeaveLabel3");
            }
        }

        public string LeaveLabel4
        {
            get { return _leaveLabel4; }
            set
            {
                _leaveLabel4 = value;
                NotifyPropertyChanged("LeaveLabel4");
            }
        }

        public string LeaveLabel5
        {
            get { return _leaveLabel5; }
            set
            {
                _leaveLabel5 = value;
                NotifyPropertyChanged("LeaveLabel5");
            }
        }

        public string LeaveLabel6
        {
            get { return _leaveLabel6; }
            set
            {
                _leaveLabel6 = value;
                NotifyPropertyChanged("LeaveLabel6");
            }
        }

        public string LeaveLabel7
        {
            get { return _leaveLabel7; }
            set
            {
                _leaveLabel7 = value;
                NotifyPropertyChanged("LeaveLabel7");
            }
        }

        public string LeaveLabel8
        {
            get { return _leaveLabel8; }
            set
            {
                _leaveLabel8 = value;
                NotifyPropertyChanged("LeaveLabel8");
            }
        }

        public string LeaveLabel9
        {
            get { return _leaveLabel9; }
            set
            {
                _leaveLabel9 = value;
                NotifyPropertyChanged("LeaveLabel9");
            }
        }

        public string LeaveLabel10
        {
            get { return _leaveLabel10; }
            set
            {
                _leaveLabel10 = value;
                NotifyPropertyChanged("LeaveLabel10");
            }
        }

        public string LeaveLabel11
        {
            get { return _leaveLabel11; }
            set
            {
                _leaveLabel11 = value;
                NotifyPropertyChanged("LeaveLabel11");
            }
        }

        public string LeaveLabel12
        {
            get { return _leaveLabel12; }
            set
            {
                _leaveLabel12 = value;
                NotifyPropertyChanged("LeaveLabel12");
            }
        }

        public int NbDay
        {
            get { return _nbDay; }
            set
            {
                _nbDay = value;
                NotifyPropertyChanged("NbDay");
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
