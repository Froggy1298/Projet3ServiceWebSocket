using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Server.Modele.JsonClass;

namespace Server.Modele
{
    class BdService
    {
        private MySqlConnection _connectionMySql;
        
        public BdService(SqlConnection SqlSettings)
        {
            string stringConnection = "SERVER=" + SqlSettings.Server + ";DATABASE=" + SqlSettings.Database + ";UID=" + SqlSettings.User + ";PASSWORD=" + SqlSettings.Password + ";PORT=" + SqlSettings.Port;
            _connectionMySql = new MySqlConnection(stringConnection);
        }


       
        public List<string> SelectAllUserMessages(string ConnectedUser)
        {
            List<string> messages = new List<string>();
            MySqlCommand stmt = new MySqlCommand($"SELECT * FROM db_dev_2031887.messagesclient WHERE nomClient = @nomClient;", _connectionMySql);
            stmt.Parameters.AddWithValue("@nomClient", ConnectedUser);

            using (_connectionMySql)
            {
                _connectionMySql.Open();

                using (MySqlDataReader dataBdReader = stmt.ExecuteReader())
                {
                    while (dataBdReader.Read())
                    {
                        messages.Add((string)dataBdReader["messageClient"]);
                    }
                }
            }
            return messages;

            /*if (DeleteAllMessagesUser(ConnectedUser))
                return messages;
            else
                return null;*/

        }
        public bool DeleteAllMessagesUser(string userName)
        {
            try
            {
                MySqlCommand stmt = new MySqlCommand($"DELETE FROM db_dev_2031887.messagesclient WHERE nomClient = '{userName}';", _connectionMySql);

                stmt.ExecuteNonQuery();

                return true;

            }
            catch (MySqlException exception)
            {
                MessageBox.Show(exception.Message + "\n" + exception.Source + "\n" + exception.Message);
                return false; ;
            }

        }
        public bool AddMessageUser(string userName, string message)
        {
            MySqlCommand stmt = new MySqlCommand($"INSERT INTO db_dev_2031887.messagesclient VALUE (@userName,@message)", _connectionMySql);
            stmt.Parameters.AddWithValue("@userName", userName);
            stmt.Parameters.AddWithValue("@message", message);


            using (_connectionMySql)
            {
                _connectionMySql.Open();
                stmt.ExecuteNonQuery();
               
            }
            return true;
        }




    }
}
