using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Barcelone___OGTS.Model
{
    class CETOperation : INotifyPropertyChanged
    {
         #region Constructor
        public CETOperation(String Date, String OpType, String LeaveType, String LeaveLabel, String CETBefore, String CETAfter)
        {
            _date = Date;
            _opType = OpType;
            _leaveType = LeaveType;
            _leaveLabel = LeaveLabel;
            _cETBefore = CETBefore;
            _cETAfter = CETAfter;
        }
        #endregion

        #region Getters and Setters
        private String _date;
        private String _opType;
        private String _leaveType;
        private String _leaveLabel;
        private String _cETBefore;
        private String _cETAfter;

        public string Date
        {
            get { return _date; }
            set
            {
                _date = value;
                NotifyPropertyChanged("Date");
            }
        }

        public string OpType
        {
            get { return _opType; }
            set
            {
                _opType = value;
                NotifyPropertyChanged("OpType");
            }
        }

        public string LeaveType
        {
            get { return _leaveType; }
            set
            {
                _leaveType = value;
                NotifyPropertyChanged("LeaveType");
            }
        }

        public string LeaveLabel
        {
            get { return _leaveLabel; }
            set
            {
                _leaveLabel = value;
                NotifyPropertyChanged("LeaveLabel");
            }
        }

        public string CETBefore
        {
            get { return _cETBefore; }
            set
            {
                _cETBefore = value;
                NotifyPropertyChanged("CETBefore");
            }
        }

        public string CETAfter
        {
            get { return _cETAfter; }
            set
            {
                _cETAfter = value;
                NotifyPropertyChanged("CETAfter");
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
