using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Windows;

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
    }
}
