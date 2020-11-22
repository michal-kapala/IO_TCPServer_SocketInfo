using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace IO_TCPServer_API
{
    class TextProtocol
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
            EXCEPTION
        };

        static string lastDcedClient = null;
        public static string LastDCedClient => lastDcedClient ?? "null";
        const string help = @"#help       display this list
#signin      sign in to chat
#register    register on chat
#disconnect  terminate connection to chat
> ";
        public static Status Process(TcpClient client, string command, string login, string password)
        {
            NetworkStream stream = client.GetStream();
            switch (command)
            {
                default:
                    return Status.MESSAGE;
                case "#help":
                    SendHelpMsg(client);
                    ConsoleLogger.Log("Sent help", LogSource.TEXT, LogLevel.INFO);
                    return Status.HELP;
                case "#signin":
                    if(DBManager.FindUser(login, password))
                    {
                        ConsoleLogger.Log("User " + login + " connected to chat.", LogSource.TEXT, LogLevel.INFO);
                        return Status.SIGNED_IN;
                    }
                    else
                    {
                        ConsoleLogger.Log("Wrong credentials", LogSource.TEXT, LogLevel.ERROR);
                        return Status.WRONG_CREDENTIALS;
                    }
                case "#register":
                    try
                    {
                        if (DBManager.AddUser(login, password))
                        {
                            ConsoleLogger.Log("User " + login + " already exists", LogSource.TEXT, LogLevel.ERROR);
                            return Status.EXISTS;
                        }
                        else
                        {
                            ConsoleLogger.Log("User " + login + " registered", LogSource.TEXT, LogLevel.INFO);
                            return Status.REGISTERED;
                        }
                    }
                    catch(Exception ex)
                    {
                        ConsoleLogger.Log("Database exception:\n" + ex.ToString(), LogSource.DB, LogLevel.ERROR);
                        return Status.EXCEPTION;
                    }
                case "#disconnect":
                    lastDcedClient = GetSocketInfo(client, false);
                    ConsoleLogger.Log("Client " + lastDcedClient + " disconnected from session", LogSource.TEXT, LogLevel.INFO);
                    client.Close();
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

        public static void SendHelpMsg(TcpClient client)
        {
            byte[] helpMsg = System.Text.Encoding.Unicode.GetBytes(help);
            foreach (byte b in helpMsg) client.GetStream().WriteByte(b);
        }

        public static KeyValuePair<string, string> GetCredentials(TcpClient client)
        {
            int bufSize = 1024;
            byte[] loginBuf = new byte[bufSize];
            byte[] passwordBuf = new byte[bufSize];
            string loginMsg = "login: ";
            byte[] loginByte = System.Text.Encoding.Unicode.GetBytes(loginMsg);
            foreach (byte b in loginByte) client.GetStream().WriteByte(b);
            Helper.ReadNetStream(client, loginBuf, 2, bufSize);
            string passwordMsg = "password: ";
            byte[] passwordByte = System.Text.Encoding.Unicode.GetBytes(passwordMsg);
            foreach (byte b in passwordByte) client.GetStream().WriteByte(b);
            client.GetStream().Read(passwordBuf, 0, bufSize);
            string login = Helper.MakeString(loginBuf);
            string password = Helper.MakeString(passwordBuf);

            return new KeyValuePair<string, string>(login, password);
        }
    }
}
