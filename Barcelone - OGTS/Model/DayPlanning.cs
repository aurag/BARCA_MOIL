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
        public DayPlanning(Boolean IsAWorkDay, String LeaveLabelJanuary, String LeaveLabelFebruary, String LeaveLabelMarch, 
                           String LeaveLabelApril, String LeaveLabelMai, String LeaveLabelJune, String LeaveLabelJuly, 
                           String LeaveLabelAugust, String LeaveLabelSeptember,  String LeaveLabelOctober,  String LeaveLabelNovember,
                           String LeaveLabelDecember, int NbDay)
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
        private String _leaveLabelJanuary;
        private String _leaveLabelFebruary;
        private String _leaveLabelMarch;
        private String _leaveLabelApril;
        private String _leaveLabelMai;
        private String _leaveLabelJune;
        private String _leaveLabelJuly;
        private String _leaveLabelAugust;
        private String _leaveLabelSeptember;
        private String _leaveLabelOctober;
        private String _leaveLabelNovember;
        private String _leaveLabelDecember;
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

        public String LeaveLabelJanuary
        {
            get { return _leaveLabelJanuary; }
            set
            {
                _leaveLabelJanuary = value;
                NotifyPropertyChanged("LeaveLabelJanuary");
            }
        }

        public String LeaveLabelFebruary
        {
            get { return _leaveLabelFebruary; }
            set
            {
                _leaveLabelFebruary = value;
                NotifyPropertyChanged("LeaveLabelFebruary");
            }
        }

        public String LeaveLabelMarch
        {
            get { return _leaveLabelMarch; }
            set
            {
                _leaveLabelMarch = value;
                NotifyPropertyChanged("LeaveLabelMarch");
            }
        }

        public String LeaveLabelApril
        {
            get { return _leaveLabelApril; }
            set
            {
                _leaveLabelApril = value;
                NotifyPropertyChanged("LeaveLabelApril");
            }
        }

        public String LeaveLabelMai
        {
            get { return _leaveLabelMai; }
            set
            {
                _leaveLabelMai = value;
                NotifyPropertyChanged("LeaveLabelMai");
            }
        }

        public String LeaveLabelJune
        {
            get { return _leaveLabelJune; }
            set
            {
                _leaveLabelJune = value;
                NotifyPropertyChanged("LeaveLabelJune");
            }
        }

        public String LeaveLabelJuly
        {
            get { return _leaveLabelJuly; }
            set
            {
                _leaveLabelJuly = value;
                NotifyPropertyChanged("LeaveLabelJuly");
            }
        }

        public String LeaveLabelAugust
        {
            get { return _leaveLabelAugust; }
            set
            {
                _leaveLabelAugust = value;
                NotifyPropertyChanged("LeaveLabelAugust");
            }
        }

        public String LeaveLabelSeptember
        {
            get { return _leaveLabelSeptember; }
            set
            {
                _leaveLabelSeptember = value;
                NotifyPropertyChanged("LeaveLabelSeptember");
            }
        }

        public String LeaveLabelOctober
        {
            get { return _leaveLabelOctober; }
            set
            {
                _leaveLabelOctober = value;
                NotifyPropertyChanged("LeaveLabelOctober");
            }
        }

        public String LeaveLabelNovember
        {
            get { return _leaveLabelNovember; }
            set
            {
                _leaveLabelNovember = value;
                NotifyPropertyChanged("LeaveLabelNovember");
            }
        }

        public String LeaveLabelDecember
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
