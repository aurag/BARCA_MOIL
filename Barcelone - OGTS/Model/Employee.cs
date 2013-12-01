using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Barcelone___OGTS.Model
{
    class Employee
    {
        #region Fields
        private string _lastname;

        public string Lastname
        {
            get { return _lastname; }
            set { _lastname = value; }
        }

        private string _firstname;

        public string Firstname
        {
            get { return _firstname; }
            set { _firstname = value; }
        }

        private string _trigram;

        public string Trigram
        {
            get 
            {
                _trigram = Firstname.Substring(0, 1) + Lastname.Substring(0, 2);
                return _trigram; 
            }
            set { _trigram = value; }
        }

        private string _adress;

        public string Adress
        {
            get { return _adress; }
            set { _adress = value; }
        }

        private string _city;

        public string City
        {
            get { return _city; }
            set { _city = value; }
        }
        private string _postalCode;

        public string PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }
        private string _familySituation;

        public string FamilySituation
        {
            get { return _familySituation; }
            set { _familySituation = value; }
        }
        private string _matricule;

        public string Matricule
        {
            get { return _matricule; }
            set { _matricule = value; }
        }
        private string _status;

        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }
        private string _statusPosition;

        public string StatusPosition
        {
            get { return _statusPosition; }
            set { _statusPosition = value; }
        }
        private string _socialNumber;

        public string SocialNumber
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
        private string _employeeId;

        public string EmployeeId
        {
            get { return _employeeId; }
            set { _employeeId = value; }
        }
        private string _entityId;

        public string EntityId
        {
            get { return _entityId; }
            set { _entityId = value; }
        }

        private Boolean _isRh;
        public Boolean IsRH
        {
            get
            {
                return _isRh;
            }
            set { _isRh = value; }
        }

        #endregion

    }
}
