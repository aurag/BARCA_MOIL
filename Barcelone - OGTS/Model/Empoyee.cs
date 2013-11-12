﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Barcelone___OGTS.Model
{
    class Employee
    {
        #region Fields
        private String _lastname;

        public String Lastname
        {
            get { return _lastname; }
            set { _lastname = value; }
        }

        private String _firstname;

        public String Firstname
        {
            get { return _firstname; }
            set { _firstname = value; }
        }

        private String _adress;

        public String Adress
        {
            get { return _adress; }
            set { _adress = value; }
        }

        private String _city;

        public String City
        {
            get { return _city; }
            set { _city = value; }
        }
        private String _postalCode;

        public String PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }
        private String _familySituation;

        public String FamilySituation
        {
            get { return _familySituation; }
            set { _familySituation = value; }
        }
        private String _matricule;

        public String Matricule
        {
            get { return _matricule; }
            set { _matricule = value; }
        }
        private String _status;

        public String Status
        {
            get { return _status; }
            set { _status = value; }
        }
        private String _statusPosition;

        public String StatusPosition
        {
            get { return _statusPosition; }
            set { _statusPosition = value; }
        }
        private String _socialNumber;

        public String SocialNumber
        {
            get { return _socialNumber; }
            set { _socialNumber = value; }
        }
        private DateTime _arrivalDate;

        public DateTime ArrivalDate
        {
            get { return _arrivalDate; }
            set { _arrivalDate = value; }
        }
        private String _employeeId;

        public String EmployeeId
        {
            get { return _employeeId; }
            set { _employeeId = value; }
        }
        private String _entityId;

        public String EntityId
        {
            get { return _entityId; }
            set { _entityId = value; }
        }

        #endregion

    }
}
