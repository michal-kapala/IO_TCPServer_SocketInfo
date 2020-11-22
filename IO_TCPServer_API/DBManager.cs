using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace IO_TCPServer_API
{
    public class DBManager
    {
        static SQLiteConnection connection = new SQLiteConnection("Data Source=database.sqlite3");

        public static void Connect()
        {
            connection.Open();
            ConsoleLogger.Log("Connected to database", LogSource.SERVER, LogLevel.INFO);
        }

        public static List<List<string>> GetQueryResults(string query)
        {
            List<List<string>> result = new List<List<string>>();
            SQLiteCommand command = new SQLiteCommand(query, connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                List<string> entry = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                    entry.Add(reader[i].ToString());
                result.Add(entry);
            }
            reader.Close();
            reader.Dispose();
            command.Dispose();
            return result;
        }

        public static bool FindUser(string login, string password)
        {
            List<List<string>> queryResult = GetQueryResults("SELECT login FROM users WHERE login=" + login);
            return queryResult != null ? true : false;
        }

        public static bool AddUser(string login, string password)
        {
            if (FindUser(login, password)) return true;
            SQLiteCommand insertUser = new SQLiteCommand("INSERT INTO users (login, password) VALUES(" + login + "," + password + ")", connection);
            try
            {
                insertUser.ExecuteNonQuery();
                ConsoleLogger.Log("User " + login + " registered", LogSource.DB, LogLevel.INFO);
            }
            catch
            {
                ConsoleLogger.Log("Database error on user insert", LogSource.DB, LogLevel.ERROR);
                throw new Exception("Database error on user insert");
            }
            return false;
        }
    }
}
