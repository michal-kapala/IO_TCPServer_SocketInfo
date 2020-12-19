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
        public static SQLiteConnection connection = new SQLiteConnection(@"Data Source=database.sqlite3");

        public static void Connect()
        {
            connection.Open();
            string sqlCreateTable = @"CREATE TABLE IF NOT EXISTS users(
id INTEGER PRIMARY KEY,	
login TEXT,
password TEXT
);";
            SQLiteCommand createUsers = new SQLiteCommand(sqlCreateTable, connection);
            try
            {
                createUsers.ExecuteNonQuery();
                ConsoleLogger.Log("Create table users statement executed", LogSource.DB, LogLevel.DEBUG);
            }
            catch (Exception ex)
            {
                ConsoleLogger.Log("DB connection exception:\n" + ex.ToString(), LogSource.DB, LogLevel.ERROR);
            }
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

        public static bool FindUser(string login)
        {
            try
            {
                List<List<string>> queryResult = GetQueryResults("SELECT login FROM users WHERE login='" + login + "'");
                ConsoleLogger.Log("FindUser: " + login + " query executed", LogSource.DB, LogLevel.DEBUG);
                return queryResult.Count > 0 ? true : false;
            }
            catch(Exception ex)
            {
                ConsoleLogger.Log("FindUser exception:\n" + ex.ToString(), LogSource.DB, LogLevel.ERROR);
            }
            return false;
        }

        public static bool AddUser(string login, string password)
        {
            if (FindUser(login)) return false;
            SQLiteCommand insertUser = new SQLiteCommand("INSERT INTO users (login, password) VALUES('" + login + "','" + Helper.MakeSHA256Hash(password) + "')", connection);
            try
            {
                insertUser.ExecuteNonQuery();
                ConsoleLogger.Log("User " + login + " registered", LogSource.DB, LogLevel.DEBUG);
                return true;
            }
            catch(Exception ex)
            {
                ConsoleLogger.Log("AddUser exception:\n" + ex.ToString(), LogSource.DB, LogLevel.ERROR);
            }
            return false;
        }

        public static bool ValidateUser(string login, string pass)
        {
            try
            {
                List<List<string>> result = GetQueryResults("SELECT login, password FROM users WHERE login='" + login + "' AND password='" + Helper.MakeSHA256Hash(pass) + "'");
                return (result.Count == 1) ? true : false;
            }
            catch(Exception ex)
            {
                ConsoleLogger.Log("ValidateUser exception:\n" + ex.ToString(), LogSource.DB, LogLevel.ERROR);
            }
            return false;
        }
    }
}
