using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Linq;

namespace IO_TCPServer_API
{
    public class TextProtocol
    {
        public enum Status
        {
            SIGNED_IN,
            WRONG_CREDENTIALS,
            REGISTERED,
            EXISTS,
            HELP,
            DISCONNECTED,
            MESSAGE,
            EXCEPTION,
            CHAT_JOIN,
            CHAT_LEAVE
        };

        static string lastDcedClient = null;
        public static string LastDCedClient => lastDcedClient ?? "null";
        const string help = @"#help       display this list
#signin      sign in to chat
#register    register on chat
#disconnect  terminate connection to chat
> ";
        public static string Help => help;

        public static Status Process(SimpleTCPServer server, TcpClient client, string command, string login, string password)
        {
            NetworkStream stream = client.GetStream();
            switch (command)
            {
                default:
                    return Status.MESSAGE;
                case "#help":
                    SendMsgToClient(client, help);
                    ConsoleLogger.Log("Sent help", LogSource.TEXT, LogLevel.INFO);
                    return Status.HELP;
                case "#chat":
                    User user = server.UserManager.GetUser(login);
                    if(user == null)
                    {
                        SendMsgToClient(client, "You're logged out, please sign in first.\n");
                        return Status.WRONG_CREDENTIALS;
                    }
                    user.ChatMode = !user.ChatMode;
                    if (user.ChatMode)
                    {
                        ConsoleLogger.Log("User " + login + " joined chat", LogSource.USER, LogLevel.INFO);
                        SendMsgToClient(client, "You've joined the chat.\n");
                        return Status.CHAT_JOIN;
                    }
                    else
                    {
                        ConsoleLogger.Log("User " + login + "left chat", LogSource.USER, LogLevel.INFO);
                        SendMsgToClient(client, "You've left the chat.\n");
                        return Status.CHAT_LEAVE;
                    }
                case "#signin":
                    if(DBManager.ValidateUser(login, password))
                    {
                        try
                        {
                            server.UserManager.SignIn(client, login, password);
                        }
                        catch(Exception ex)
                        {
                            ConsoleLogger.Log("Sign-in exception:\n" + ex.ToString(), LogSource.TEXT, LogLevel.ERROR);
                        }
                        ConsoleLogger.Log("User " + login + " signed in.", LogSource.TEXT, LogLevel.INFO);
                        SendMsgToClient(client, "You have been logged in.\n");
                        return Status.SIGNED_IN;
                    }
                    else
                    {
                        ConsoleLogger.Log("Wrong credentials from " + GetSocketInfo(client, false), LogSource.TEXT, LogLevel.INFO);
                        SendMsgToClient(client, "Wrong login or password, please try again.\n");
                        return Status.WRONG_CREDENTIALS;
                    }
                case "#register":
                        if (DBManager.AddUser(login, password))
                        {
                            ConsoleLogger.Log("User " + login + " registered", LogSource.TEXT, LogLevel.INFO);
                            SendMsgToClient(client, "You've been successfully registered.\n");
                            return Status.REGISTERED;
                        }
                        else
                        { 
                            ConsoleLogger.Log("User " + login + " already exists", LogSource.TEXT, LogLevel.ERROR);
                            SendMsgToClient(client, "Login is unavailable, please use a different one.\n");
                            return Status.EXISTS;
                        }
                case "#disconnect":
                    lastDcedClient = GetSocketInfo(client, false);
                    ConsoleLogger.Log("Client " + lastDcedClient + " disconnected from session", LogSource.TEXT, LogLevel.INFO);
                    SendMsgToClient(client, "You have disconnected from server.\n");
                    return Status.DISCONNECTED;
            }
        }

        public static string GetSocketInfo(TcpClient client, bool bFull)
        {
            IPEndPoint clientEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
            string clientSocket = clientEndPoint.Address.ToString() + ":" + clientEndPoint.Port.ToString();
            string socketInfo = "client: " + clientSocket;
            if (bFull)
            {
                socketInfo += "\n";
                IPEndPoint serverEndPoint = (IPEndPoint)client.Client.LocalEndPoint;
                string serverSocket = serverEndPoint.Address.ToString() + ":" + serverEndPoint.Port.ToString();
                socketInfo += "server: " + serverSocket + "\n";
            }
            return socketInfo;
        }

        public static void SendMsgToClient(TcpClient client, string msg)
        {
            byte[] buffer = System.Text.Encoding.Unicode.GetBytes(msg);
            foreach (byte b in buffer) client.GetStream().WriteByte(b);
        }

        public static void BroadcastMessages(SimpleTCPServer server)
        {
            string clear = String.Concat(Enumerable.Repeat("\n", 100));
            byte[] buffer = new byte[1024];
            foreach (User u in server.UserManager.activeUsers)
            {
                u.Client.GetStream().Write(System.Text.Encoding.Unicode.GetBytes(clear), 0, clear.Length);
                foreach (string message in server.messages)
                {
                    buffer = System.Text.Encoding.Unicode.GetBytes(message + "\n");
                    u.Client.GetStream().Write(buffer, 0, buffer.Length);
                }
                buffer = System.Text.Encoding.Unicode.GetBytes("> ");
                u.Client.GetStream().Write(buffer, 0, buffer.Length);
            }
        }
    }
}
