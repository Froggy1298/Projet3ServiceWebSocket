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
            _connectionMySql.Open();
        }


        private bool CloseMySqlConnection()
        {
            try
            {
                _connectionMySql.Close();
                return true;
            }
            catch(MySqlException exception)
            {
                MessageBox.Show(exception.Message + "\n" + exception.Source + "\n" + exception.Message);
                return false;
            }
        }
        private bool OpenMySqlConnection()
        {
            try
            {
                CloseMySqlConnection();
                _connectionMySql.Open();
                return true;
            }
            catch (MySqlException exception)
            {
                MessageBox.Show("--" + exception.Message + "\n\n--" + exception.Source + "\n\n--" + exception.Message);
                return false;
            }

        }
        public List<string> SelectAllUserMessages(string ConnectedUser)
        {
            try
            {
                if (OpenMySqlConnection())
                {

                    List<string> messages = new List<string>();

                    MySqlCommand stmt = new MySqlCommand($"SELECT * FROM db_dev_2031887.messagesclient WHERE nomClient = '{ConnectedUser}';", _connectionMySql);

                    MySqlDataReader dataBdReader = stmt.ExecuteReader();

                    while (dataBdReader.Read())
                    {
                        messages.Add((string)dataBdReader["messageClient"]);
                    }
                    dataBdReader.Close();
                    return messages;
                    /*if (DeleteAllMessagesUser(ConnectedUser))
                        return messages;
                    else
                       return null;*/
                }
                else
                return null;
            }
            catch (MySqlException exception)
            {
                MessageBox.Show(exception.Message + "\n" + exception.Source + "\n" + exception.Message);
                return null; ;
            }
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
            try
            {
                if (OpenMySqlConnection())
                {
                    MySqlCommand stmt = new MySqlCommand($"INSERT INTO db_dev_2031887.messagesclient VALUE ('{userName}','{message}')", _connectionMySql);
                    stmt.ExecuteNonQuery();
                    CloseMySqlConnection();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (MySqlException exception)
            {
                MessageBox.Show(exception.Message + "\n" + exception.Source + "\n" + exception.Message);
                return false; ;
            }
           
        }




    }
}
