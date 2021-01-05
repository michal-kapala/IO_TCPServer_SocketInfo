using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using JsonProtocol;

namespace IO_TCPServer_API
{
    public class UserManager
    {
        public List<User> activeUsers;

        public UserManager()
        {
            activeUsers = new List<User>();
        }

        public static KeyValuePair<string, string> GetCredentials(TcpClient client)
        {
            int bufSize = 1024;
            byte[] loginBuf = new byte[bufSize];
            byte[] passwordBuf = new byte[bufSize];
            string loginMsg = "login: ";
            byte[] loginByte = Encoding.Unicode.GetBytes(loginMsg);
            foreach (byte b in loginByte) client.GetStream().WriteByte(b);
            //Helper.ReadNetStream(client, loginBuf, 2, bufSize);
            client.GetStream().Read(loginBuf, 0, bufSize);
            string passwordMsg = "password: ";
            byte[] passwordByte = Encoding.Unicode.GetBytes(passwordMsg);
            foreach (byte b in passwordByte) client.GetStream().WriteByte(b);
            client.GetStream().Read(passwordBuf, 0, bufSize);
            string login = Helper.MakeString(loginBuf);
            string password = Helper.MakeString(passwordBuf);

            return new KeyValuePair<string, string>(login, password);
        }

        public bool SignIn(TcpClient client, string login, string password)
        {
            Credentials cr = new Credentials(login, password);
            foreach(User u in activeUsers)
            {
                if (u.Login == login) return false;
                
            }
            activeUsers.Add(new User(client, cr));
            return true;
        }

        public void SignOut(string login)
        {
            foreach (User u in activeUsers)
            {
                if (!u.SignedIn)
                {
                    ConsoleLogger.Log("Error: Found signed-out active user" + login, LogSource.USER, LogLevel.ERROR);
                    activeUsers.Remove(u);
                }
                if (u.Login == login) activeUsers.Remove(u);
            }
        }

        public User GetUser(string login)
        {
            foreach (User u in activeUsers) if (u.Login == login) return u;
            ConsoleLogger.Log("User " + login + " not found", LogSource.USER, LogLevel.INFO);
            return null;
        }

        public User GetUser(TcpClient client)
        {
            foreach(User u in activeUsers)
            {
                if (u.Client == client)
                    return u;
            }
            return null;
        }

        public SimpleTCPServer.Status HandleWrongCredentials(SimpleTCPServer server, TcpClient client, string command)
        {
            try
            {
                TextProtocol.SendMsg(client, "Please register first:\n");
                command = "#r";
                KeyValuePair<string, string> creds = GetCredentials(client);
                return TextProtocol.ProcessCommand(server, client, command, creds.Key, creds.Value);
            }
            catch(Exception ex)
            {
                ConsoleLogger.Log("Exception:\n" + ex.ToString(), LogSource.USER, LogLevel.ERROR);
                return SimpleTCPServer.Status.EXCEPTION;
            }
        }
    }
}
