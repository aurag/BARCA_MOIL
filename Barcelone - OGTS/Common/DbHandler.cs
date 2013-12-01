using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Windows;
using Barcelone___OGTS.Model;

namespace Barcelone___OGTS.Common
{
    class DbHandler
    {
        private static DbHandler _instance = null;
        private static readonly object _padlock = new object();
        private static NpgsqlConnection _connection;

        DbHandler()
        {
            string serverName = "bnf.sigl.epita.fr";
            string serverPort = "5432";
            string user = "Barcelone";
            string password = "RB56fx";
            string databaseName = "Barcelone";
            string connection = string.Format("Server={0}; Port={1}; User Id={2}; Password={3}; Database={4}", serverName, serverPort, user, password, databaseName);
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(connection);
                _connection = conn;
            }
            catch(Exception e)
            {
                MessageBox.Show("Erreur de création de la connexion à la base de données", "Erreur");
                Console.WriteLine("Connection failed to the database :");
                Console.WriteLine(e.Message);
            }
        }


        public Boolean checkIfRh(string id_employee)
        {
            DbHandler.Instance.OpenConnection();
            NpgsqlDataReader result;

            result = DbHandler.Instance.ExecSQL(@"select id_employee, id_employee_rh from public.employee
                                                                   WHERE public.employee.id_employee_rh=" + id_employee + ";");

            Boolean res = false;
            if (result != null)
            {
                while (result.Read())
                {
                    res = true;
                }
            }

            DbHandler.Instance.CloseConnection();
            return res;
        }

        public static DbHandler Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new DbHandler();
                    }
                    return _instance;
                }
            }
        }

        public void OpenConnection()
        {
            try
            {
                _connection.Open();
            }
            catch (Exception e)
            {
                //MessageBox.Show("Erreur de création de la connexion à la base de données", "Erreur");
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
               
            }
        }

        public void CloseConnection()
        {
            try
            {
                _connection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Erreur de terminaison de la connexion à la base de données", "Erreur");
                Console.WriteLine(e.Message);
            }
        }

        public NpgsqlDataReader ExecSQL(string sql)
        {
            NpgsqlDataReader result = null;
            try
            {
                NpgsqlCommand command = new NpgsqlCommand(sql, _connection);
                result = command.ExecuteReader();

                // Output rows
                //while (result.Read())
                  //  Console.WriteLine("test : '{0}'", result[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        // Récupération de la liste de tous les congés pour l'utilisateur actuel
        public List<DayOff> getDaysOffList()
        {
            DbHandler.Instance.OpenConnection();
            List<DayOff> _daysOff = new List<DayOff>();

            try
            {
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL(string.Format(@"select start_date, end_date, creation_date, type, title, status, 
                                                                   employee_commentary, superior_commentary, validation_date
                                                                   from public.dayoff, public.dayofftype
                                                                   WHERE public.dayoff.id_day_off_type = public.dayofftype.id_day_off_type
                                                                   AND public.dayoff.id_employee={0} ORDER BY start_date;", UserSession.Instance.User.Employee.EmployeeId));

                if (result != null)
                {
                    while (result.Read())
                    {
                        DayOff dayOff = new DayOff()
                        {
                            StartDate = result[0].ToString().Substring(0, 10),
                            EndDate = result[1].ToString().Substring(0, 10),
                            CreationDate = DateTime.Today.ToShortDateString(),
                            Type = result[3].ToString(),
                            Title = result[4].ToString(),
                            Status = result[5].ToString(),
                            CommentSal = result[6].ToString(),
                            CommentRh = result[7].ToString(),
                            DateRh = result[8].ToString(),
                            IdEmployee = UserSession.Instance.User.Employee.EmployeeId
                        };

                        _daysOff.Add(dayOff);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur : " + e.Message);
            }
            finally
            {
                DbHandler.Instance.CloseConnection();
            }

            return _daysOff;
        }

        // Récupération de la liste de tous les congés pour l'utilisateur actuel avec un statut donné en paramètre
        public List<DayOff> getDaysOffList(string status)
        {
            DbHandler.Instance.OpenConnection();
            List<DayOff> _daysOff = new List<DayOff>();

            try
            {
                NpgsqlDataReader result = DbHandler.Instance.ExecSQL(string.Format(@"select start_date, end_date, creation_date, type, title, status, 
                                                                   employee_commentary, superior_commentary, validation_date
                                                                   from public.dayoff INNER JOIN public.dayofftype
                                                                   ON (public.dayoff.id_day_off_type = public.dayofftype.id_day_off_type) where public.dayoff.status = {0}
                                                                   AND public.dayoff.id_employee={1} ORDER BY start_date;", status, UserSession.Instance.User.Employee.EmployeeId));

                if (result != null)
                {
                    while (result.Read())
                    {
                        DayOff dayOff = new DayOff()
                        {
                            StartDate = result[0].ToString().Substring(0, 10),
                            EndDate = result[1].ToString().Substring(0, 10),
                            CreationDate = DateTime.Today.ToShortDateString(),
                            Type = result[3].ToString(),
                            Title = result[4].ToString(),
                            Status = result[5].ToString(),
                            CommentSal = result[6].ToString(),
                            CommentRh = result[7].ToString(),
                            DateRh = result[8].ToString(),
                            IdEmployee = UserSession.Instance.User.Employee.EmployeeId
                        };

                        _daysOff.Add(dayOff);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur : " + e.Message);
            }
            finally
            {
                DbHandler.Instance.CloseConnection();
            }

            return _daysOff;
        }
    }
}
