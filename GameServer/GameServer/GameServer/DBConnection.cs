using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;

namespace GameServer
{
    public class DBConnection
    {
        MySqlConnection connection;
        ServerForm Form;
        public DBConnection(ServerForm Form)
        {
            this.Form = Form;
        }

        public MySqlConnection Connect(string address, int port, string user, string pass, string database)
        {
            string connectionString = "SERVER=" + address + "; PORT=" + port + "; DATABASE=" +
            database + ";" + "UID=" + user + ";" + "PASSWORD=" + pass + "; SSL Mode=None";
            //connectionString = "SERVER=" + address + "; PORT=" + port + "; DATABASE=" +
            //database + ";" + "UID=" + user + ";" + "PWD=" + pass + ";";
            if (string.IsNullOrEmpty(pass))
            {
                connectionString = "SERVER=" + address + "; PORT=" + port + "; DATABASE=" +
                database + ";" + "UID=" + user + ";SSL Mode=None";
            }
            connection = new MySqlConnection(connectionString);
           
            return connection;
        }

        //CheckConnection
        public bool CheckConnection()
        {
            try
            {
                connection.Open();
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Form.AppendServerLogLine(ex.Message);
                return false;
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        //MessageBox.Show("Cannot connect to server.  Contact administrator");
                        Form.AppendServerLogLine("Cannot connect to database server.  Contact administrator");
                        break;
                    case 1042:
                        Form.AppendServerLogLine("Unable to connect to any of the specified MySQL hosts");
                        break;
                    case 1045:
                        Form.AppendServerLogLine("Invalid username/password, please try again");
                        //MessageBox.Show("Invalid username/password, please try again");
                        break;
                    default:
                        Form.AppendServerLogLine("database exception: "+ex.Number);
                        break;
                }
                return false;
            }
        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        //MessageBox.Show("Cannot connect to server.  Contact administrator");
                        Form.AppendServerLogLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Form.AppendServerLogLine("Invalid username/password, please try again");
                        //MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                //MessageBox.Show(ex.Message);
                return false;
            }
        }

        //Select statement
        public DataTable Query(string sql)
        {
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader dReader = cmd.ExecuteReader();
            DataTable dTable = new DataTable();
            dTable.Load(dReader);
            CloseConnection();
            return dTable;
        }

        public void NonQuery(string sql)
        {
            OpenConnection();
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {

            }
            CloseConnection();
        }


        public void Close()
        {
            connection.Close();
        }
    }
}
