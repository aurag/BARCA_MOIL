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
            _leaveLabelJanuary = LeaveLabelJanuary;
            _leaveLabelFebruary = LeaveLabelFebruary;
            _leaveLabelMarch = LeaveLabelMarch;
            _leaveLabelApril = LeaveLabelApril;
            _leaveLabelMai = LeaveLabelMai;
            _leaveLabelJune = LeaveLabelJune;
            _leaveLabelJuly = LeaveLabelJuly;
            _leaveLabelAugust = LeaveLabelAugust;
            _leaveLabelSeptember = LeaveLabelSeptember;
            _leaveLabelOctober = LeaveLabelOctober;
            _leaveLabelNovember = LeaveLabelNovember;
            _leaveLabelDecember = LeaveLabelDecember;

            _nbDay = NbDay;
        }
        #endregion

        #region Getters and Setters

        private Boolean _isAWorkDay;
        private string _leaveLabelJanuary;
        private string _leaveLabelFebruary;
        private string _leaveLabelMarch;
        private string _leaveLabelApril;
        private string _leaveLabelMai;
        private string _leaveLabelJune;
        private string _leaveLabelJuly;
        private string _leaveLabelAugust;
        private string _leaveLabelSeptember;
        private string _leaveLabelOctober;
        private string _leaveLabelNovember;
        private string _leaveLabelDecember;
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

        public string LeaveLabelJanuary
        {
            get { return _leaveLabelJanuary; }
            set
            {
                _leaveLabelJanuary = value;
                NotifyPropertyChanged("LeaveLabelJanuary");
            }
        }

        public string LeaveLabelFebruary
        {
            get { return _leaveLabelFebruary; }
            set
            {
                _leaveLabelFebruary = value;
                NotifyPropertyChanged("LeaveLabelFebruary");
            }
        }

        public string LeaveLabelMarch
        {
            get { return _leaveLabelMarch; }
            set
            {
                _leaveLabelMarch = value;
                NotifyPropertyChanged("LeaveLabelMarch");
            }
        }

        public string LeaveLabelApril
        {
            get { return _leaveLabelApril; }
            set
            {
                _leaveLabelApril = value;
                NotifyPropertyChanged("LeaveLabelApril");
            }
        }

        public string LeaveLabelMai
        {
            get { return _leaveLabelMai; }
            set
            {
                _leaveLabelMai = value;
                NotifyPropertyChanged("LeaveLabelMai");
            }
        }

        public string LeaveLabelJune
        {
            get { return _leaveLabelJune; }
            set
            {
                _leaveLabelJune = value;
                NotifyPropertyChanged("LeaveLabelJune");
            }
        }

        public string LeaveLabelJuly
        {
            get { return _leaveLabelJuly; }
            set
            {
                _leaveLabelJuly = value;
                NotifyPropertyChanged("LeaveLabelJuly");
            }
        }

        public string LeaveLabelAugust
        {
            get { return _leaveLabelAugust; }
            set
            {
                _leaveLabelAugust = value;
                NotifyPropertyChanged("LeaveLabelAugust");
            }
        }

        public string LeaveLabelSeptember
        {
            get { return _leaveLabelSeptember; }
            set
            {
                _leaveLabelSeptember = value;
                NotifyPropertyChanged("LeaveLabelSeptember");
            }
        }

        public string LeaveLabelOctober
        {
            get { return _leaveLabelOctober; }
            set
            {
                _leaveLabelOctober = value;
                NotifyPropertyChanged("LeaveLabelOctober");
            }
        }

        public string LeaveLabelNovember
        {
            get { return _leaveLabelNovember; }
            set
            {
                _leaveLabelNovember = value;
                NotifyPropertyChanged("LeaveLabelNovember");
            }
        }

        public string LeaveLabelDecember
        {
            get { return _leaveLabelDecember; }
            set
            {
                _leaveLabelDecember = value;
                NotifyPropertyChanged("LeaveLabelDecember");
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
