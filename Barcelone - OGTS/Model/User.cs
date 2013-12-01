using Barcelone___OGTS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Barcelone___OGTS.Model
{
    class User
    {
        private Employee _employee;
        private string _userId;

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        private string _lastConnectionDate;

        public string LastConnectionDate
        {
            get { return _lastConnectionDate; }
            set { _lastConnectionDate = value; }
        }

        private string _lastConnectionTime;

        public string LastConnectionTime
        {
            get { return _lastConnectionTime; }
            set { _lastConnectionTime = value; }
        }

        internal Employee Employee
        {
            get { return _employee; }
            set { _employee = value; }
        }
    }
}
