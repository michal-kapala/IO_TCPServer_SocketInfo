using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace IO_TCPServer_API
{
    class TextProtocol
    {
        static string lastDcedClient = null;
<<<<<<< Updated upstream
        public static string LastDCedClient => lastDcedClient != null ? lastDcedClient : "null";
        const string tip = "Use '#help' for command list\n";
        const string help = "#help\tdisplay this list\n" +
                            "#info\tdisplay socket information\n" +
                            "#disconnect\tend TCP session\n";

        public static void Process(TcpClient client, string command)
        {
            byte[] reply;
            NetworkStream stream = client.GetStream();
            switch (command)
            {
                case "help":
                case "#help":
                    reply = System.Text.Encoding.Unicode.GetBytes(help);
                    stream.Write(reply, 0, reply.Length);
                    ConsoleLogger.Log("Sent help", LogSource.TEXT);
                    break;
                case "#info":
                    string socketInfo = GetSocketInfo(client, true);
                    reply = System.Text.Encoding.Unicode.GetBytes(socketInfo);
                    stream.Write(reply, 0, reply.Length);
                    ConsoleLogger.Log("Sent socket info", LogSource.TEXT);
                    break;
                case "#disconnect":
                    lastDcedClient = GetSocketInfo(client, false);
                    ConsoleLogger.Log("Client " + lastDcedClient + " disconnected from session", LogSource.TEXT);
                    client.Close();
                    break;
                default: break;
=======
        public static string LastDCedClient => lastDcedClient ?? "null";
        const string help = @"#h        display help
#s      sign in to chat
#r      register on chat
#d      disconnect from chat
#c      enter/leave chat
> ";
        public static string Help => help;

        public static SimpleTCPServer.Status ProccessCommand(SimpleTCPServer server, TcpClient client, string command, string login, string password)
        {
            switch (command)
            {
                default:
                    return SimpleTCPServer.Status.MESSAGE;
                case "#h":
                    SendMsgToClient(client, help);
                    ConsoleLogger.Log("Sent help", LogSource.TEXT, LogLevel.INFO);
                    return SimpleTCPServer.Status.HELP;
                case "#c":
                    User user = server.UserManager.GetUser(login);
                    if(user == null)
                    {
                        SendMsgToClient(client, "You're logged out, please sign in first.\n");
                        return SimpleTCPServer.Status.WRONG_CREDENTIALS;
                    }
                    user.ChatMode = !user.ChatMode;
                    if (user.ChatMode)
                    {
                        ConsoleLogger.Log("User " + login + " joined chat", LogSource.USER, LogLevel.INFO);
                        SendMsgToClient(client, "You've joined the chat.\n");
                        return SimpleTCPServer.Status.CHAT_JOIN;
                    }
                    else
                    {
                        ConsoleLogger.Log("User " + login + "left chat", LogSource.USER, LogLevel.INFO);
                        SendMsgToClient(client, "You've left the chat.\n");
                        return SimpleTCPServer.Status.CHAT_LEAVE;
                    }
                case "#s":
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
                        return SimpleTCPServer.Status.SIGNED_IN;
                    }
                    else
                    {
                        ConsoleLogger.Log("Wrong credentials from " + GetSocketInfo(client, false), LogSource.TEXT, LogLevel.INFO);
                        SendMsgToClient(client, "Wrong login or password, please try again.\n");
                        return SimpleTCPServer.Status.WRONG_CREDENTIALS;
                    }
                case "#r":
                        if (DBManager.AddUser(login, password, true))
                        {
                            ConsoleLogger.Log("User " + login + " registered", LogSource.TEXT, LogLevel.INFO);
                            SendMsgToClient(client, "You've been successfully registered.\n");
                            return SimpleTCPServer.Status.REGISTERED;
                        }
                        else
                        { 
                            ConsoleLogger.Log("User " + login + " already exists", LogSource.TEXT, LogLevel.ERROR);
                            SendMsgToClient(client, "Login is unavailable, please use a different one.\n");
                            return SimpleTCPServer.Status.EXISTS;
                        }
                case "#d":
                    lastDcedClient = GetSocketInfo(client, false);
                    ConsoleLogger.Log("Client " + lastDcedClient + " disconnected from session", LogSource.TEXT, LogLevel.INFO);
                    SendMsgToClient(client, "You have disconnected from server.\n");
                    return SimpleTCPServer.Status.DISCONNECTED;
>>>>>>> Stashed changes
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

        public static void SendTipMsg(TcpClient client)
        {
            byte[] tipMsg = System.Text.Encoding.Unicode.GetBytes(tip);
            foreach (byte b in tipMsg) client.GetStream().WriteByte(b);
        }
    }
}
