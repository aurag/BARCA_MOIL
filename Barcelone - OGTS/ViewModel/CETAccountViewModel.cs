using Barcelone___OGTS.Common;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Barcelone___OGTS.Model;
using System.Windows.Data;
using System.ComponentModel;
using Barcelone___OGTS.View;
using Npgsql;

namespace Barcelone___OGTS.ViewModel
{
    class CETAccountViewModel : BaseViewModel
    {
         #region Commandes
        public ICommand BackCommand { get; set; }
        public ICommand GoToAddInCET { get; set; }
        public ICommand GoToLeaveRequest { get; set; }
        #endregion

        #region Properties

        private int _cETNumber;

        public int CETNumber
        {
            get { return _cETNumber; }
            set 
            { 
                _cETNumber = value;
                OnPropertyChanged("CETNumber");
            }
        }

        public ICollectionView CETOperations { get; private set; }

        #endregion

        /// <summary>
        /// constructeur
        /// </summary>
        public CETAccountViewModel()
        {
            BackCommand = new Command(param => Back(), param => true);
            GoToAddInCET = new Command(param => PushAddInCET(), param => true);
            GoToLeaveRequest = new Command(param => PushLeaveRequest(), param => true);

            // Récupération du solde actuel du CET
            getCurrentCET();

            // Creation de la liste des opérations sur le CET.
            CreateCETOPList();
            
        }

        public void getCurrentCET()
        {
            DbHandler.Instance.OpenConnection();
            String employeeId = UserSession.Instance.User.Employee.EmployeeId;

            try
            {
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL("select current_cet from public.employee where id_employee = " + employeeId + ";");

                if (result != null)
                {
                    int rows = 0;

                    while (result.Read())
                    {
                        if (rows > 0)
                            Console.WriteLine("Plus d'un retour pour la requête du CET !");
                        CETNumber = int.Parse(result[0].ToString());
                        rows++;
                    }

                }
                else
                {
                    Console.WriteLine("Erreur : CET non trouvé !");
                    CETNumber = 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur au niveau du calcul du solde du CET : " + e.Message);
            }
            finally
            {
                DbHandler.Instance.CloseConnection();
            }
        }

        /// <summary>
        /// réponse à la commande back
        /// </summary>
        private void Back()
        {
            Switcher.SwitchBack();
        }

        private void PushAddInCET()
        {
            Switcher.Switch(new AddInCET());
        }

        private void PushLeaveRequest()
        {
            Switcher.Switch(new LeaveRequestView());
        }

        /// <summary>
        /// Créé la liste des congés en attente
        /// </summary>
        private void CreateCETOPList()
        {
            var _CETOP = new List<CETOperation>
                {
                    new CETOperation("10/02/13", "Ajout", "03", "Congés supplémentaires", "2", "6"),
                    new CETOperation("02/02/13", "Retrait", "03", "Congés supplémentaires", "4", "2"),
                    new CETOperation("20/01/13", "Ajout", "02", "Congés d'ancienneté", "2", "4"),
                    new CETOperation("04/01/13", "Ajout", "02", "Congés d'ancienneté", "0", "2")
                };

            CETOperations = CollectionViewSource.GetDefaultView(_CETOP);
        }
    }
}
