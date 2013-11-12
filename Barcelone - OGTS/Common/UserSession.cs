using Barcelone___OGTS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Barcelone___OGTS.Common
{
    public sealed class UserSession
    {
        private static volatile UserSession instance;
        private static object syncRoot = new Object();
        private User _user;

        internal User User
        {
            get { return _user; }
            set { _user = value; }
        }

        private UserSession() { }

        public static UserSession Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new UserSession();
                    }
                }

                return instance;
            }
        }
    }
}
