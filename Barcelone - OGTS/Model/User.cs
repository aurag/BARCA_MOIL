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
        private String _userId;

        public String UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }


        internal Employee Employee
        {
            get { return _employee; }
            set { _employee = value; }
        }
    }
}
