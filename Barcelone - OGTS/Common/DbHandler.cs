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
            String serverName = "bnf.sigl.epita.fr";
            String serverPort = "5432";
            String user = "Barcelone";
            String password = "RB56fx";
            String databaseName = "Barcelone";
            String connection = String.Format("Server={0}; Port={1}; User Id={2}; Password={3}; Database={4}", serverName, serverPort, user, password, databaseName);
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
                MessageBox.Show("Erreur de création de la connexion à la base de données", "Erreur");
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

        public NpgsqlDataReader ExecSQL(String sql)
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

            NpgsqlDataReader result = DbHandler.Instance.ExecSQL(String.Format(@"select start_date, end_date, creation_date, type, title, status, 
                                                                   employee_commentary, superior_commentary, validation_date
                                                                   from public.dayoff, public.dayofftype
                                                                   WHERE public.dayoff.id_day_off_type = public.dayofftype.id_day_off_type
                                                                   AND public.dayoff.id_employee={0};", UserSession.Instance.User.Employee.EmployeeId));
            List<DayOff> _daysOff = null;
            if (result != null)
            {
                _daysOff = new List<DayOff>();
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

            DbHandler.Instance.CloseConnection();

            return _daysOff;
        }
    }
}
