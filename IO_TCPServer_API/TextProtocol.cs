using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Linq;

namespace IO_TCPServer_API
{
    class TextProtocol
    {
        static string lastDcedClient = null;
        public static string LastDCedClient => lastDcedClient != null ? lastDcedClient : "null";
        public const string tip = "Use '#help' for command list\n";
        const string help = @"#h        display help
#s      sign in to chat
#r      register on chat
#d      disconnect from chat
#c      enter/leave chat";

        public static SimpleTCPServer.Status ProcessCommand(SimpleTCPServer server, TcpClient client, string command, string login, string password)
        {
            switch (command)
            {
                default:
                    return SimpleTCPServer.Status.MSG_OK;
                case "#h":
                    SendMsg(client, help);
                    ConsoleLogger.Log("Sent help", LogSource.TEXT, LogLevel.INFO);
                    return SimpleTCPServer.Status.HELP;
                case "#c":
                    User user = server.UserManager.GetUser(login);
                    if(user == null)
                    {
                        SendMsg(client, "You're logged out, please sign in first.\n");
                        return SimpleTCPServer.Status.WRONG_CREDENTIALS;
                    }
                    user.ChatMode = !user.ChatMode;
                    if (user.ChatMode)
                    {
                        ConsoleLogger.Log("User " + login + " joined chat", LogSource.USER, LogLevel.INFO);
                        SendMsg(client, "You've joined the chat.\n");
                        return SimpleTCPServer.Status.CHAT_JOIN;
                    }
                    else
                    {
                        ConsoleLogger.Log("User " + login + "left chat", LogSource.USER, LogLevel.INFO);
                        SendMsg(client, "You've left the chat.\n");
                        return SimpleTCPServer.Status.CHAT_LEAVE;
                    }
                case "#s":
                    if(DBManager.ValidateUser(login, password))
                    {
                        bool bSignedIn = false;
                        try
                        {
                            bSignedIn = server.UserManager.SignIn(client, login, password);
                        }
                        catch(Exception ex)
                        {
                            ConsoleLogger.Log("Sign-in exception:\n" + ex.ToString(), LogSource.TEXT, LogLevel.ERROR);
                        }
                        if(bSignedIn)
                        {
                            ConsoleLogger.Log($"User {login} signed in.", LogSource.TEXT, LogLevel.INFO);
                            SendMsg(client, "You have been logged in.\n");
                            return SimpleTCPServer.Status.SIGNED_IN;
                        }
                        else
                        {
                            ConsoleLogger.Log($"User {login} already logged in!", LogSource.TEXT, LogLevel.ERROR);
                            SendMsg(client, "You are already in session!\n");
                            return SimpleTCPServer.Status.ALREADY_SIGNED_IN;
                        }
                    }
                    else
                    {
                        ConsoleLogger.Log("Wrong credentials from " + GetSocketInfo(client, false), LogSource.TEXT, LogLevel.INFO);
                        SendMsg(client, "Wrong login or password, please try again.\n");
                        return SimpleTCPServer.Status.WRONG_CREDENTIALS;
                    }
                case "#r":
                        if (DBManager.AddUser(login, password))
                        {
                            ConsoleLogger.Log("User " + login + " registered", LogSource.TEXT, LogLevel.INFO);
                            SendMsg(client, "You've been successfully registered.\n");
                            return SimpleTCPServer.Status.REGISTERED;
                        }
                        else
                        { 
                            ConsoleLogger.Log("User " + login + " already exists", LogSource.TEXT, LogLevel.ERROR);
                            SendMsg(client, "Login is unavailable, please use a different one.\n");
                            return SimpleTCPServer.Status.EXISTS;
                        }
                case "#d":
                    server.UserManager.SignOut(login);
                    lastDcedClient = GetSocketInfo(client, false);
                    ConsoleLogger.Log("Client " + lastDcedClient + " disconnected from session", LogSource.TEXT, LogLevel.INFO);
                    SendMsg(client, "You have disconnected from server.\n");
                    return SimpleTCPServer.Status.DISCONNECTED;
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

        public static void SendMsg(TcpClient client, string msg)
        {
            byte[] m = System.Text.Encoding.Unicode.GetBytes(msg);
            foreach (byte b in m) client.GetStream().WriteByte(b);
        }

        public static void BroadcastMessages(SimpleTCPServer server)
        {
            string clear = string.Concat(Enumerable.Repeat("\n", 100));
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