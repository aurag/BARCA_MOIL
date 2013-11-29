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
        public ICommand HomeCommand { get; set; }
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
            HomeCommand = new Command(param => PushHome(), param => true);
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

        private void PushHome()
        {
            Switcher.Switch(new HomeView());
        }

        /// <summary>
        /// Créé la liste des congés en attente
        /// </summary>
        private void CreateCETOPList()
        {

            DbHandler.Instance.OpenConnection();
            String employeeId = UserSession.Instance.User.Employee.EmployeeId;
            try
            {
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL("select action_date, action_type, type, title, nb_before, nb_after " +
                                                                     "from public.cethistory INNER JOIN public.dayofftype using (id_day_off_type)" +
                                                                     "where public.cethistory.id_employee = " + employeeId + ";");
                var _CETList = new List<CETOperation>();
                if (result != null)
                {
  
                    while (result.Read())
                    {
                        // date, OpType, LeaveType, LeaveLabel, CET before, CET after
                        var _CET = new CETOperation(result[0].ToString().Substring(0, 10), result[1].ToString(), result[2].ToString(), result[3].ToString(), result[4].ToString(), result[5].ToString());
                        _CETList.Add(_CET);
                    }
                }
                CETOperations = CollectionViewSource.GetDefaultView(_CETList);
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur au niveau de l'historique du CET : " + e.Message);
            }
            finally
            {
                DbHandler.Instance.CloseConnection();
            }
            /*
            var _CETOP = new List<CETOperation>
                {
                    new CETOperation("10/02/13", "Ajout", "03", "Congés supplémentaires", "2", "6"),
                    new CETOperation("02/02/13", "Retrait", "03", "Congés supplémentaires", "4", "2"),
                    new CETOperation("20/01/13", "Ajout", "02", "Congés d'ancienneté", "2", "4"),
                    new CETOperation("04/01/13", "Ajout", "02", "Congés d'ancienneté", "0", "2")
                };

            CETOperations = CollectionViewSource.GetDefaultView(_CETOP);
             * */
        }
    }
}
