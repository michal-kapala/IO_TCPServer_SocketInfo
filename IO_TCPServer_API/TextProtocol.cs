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
            MESSAGE
        };

        static string lastDcedClient = null;
        public static string LastDCedClient => lastDcedClient != null ? lastDcedClient : "null";
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
                    ConsoleLogger.Log("Sent help", LogSource.TEXT);
                    return Status.HELP;
                case "#signin":
                    if(true/*DBManager.findUser(login, password)*/)
                    {
                        ConsoleLogger.Log("User " + login + " connected to chat.", LogSource.TEXT);
                        return Status.SIGNED_IN;
                    }
                    else
                    {
                        ConsoleLogger.Log("Wrong credentials", LogSource.TEXT);
                        return Status.WRONG_CREDENTIALS;
                    }
                case "#register":
                    if (true/*DBManager.addUser(login, password)*/)
                    {
                        ConsoleLogger.Log("User " + login + " is already exists", LogSource.TEXT);
                        return Status.EXISTS;
                    }
                    else
                    {
                        ConsoleLogger.Log("User " + login + " registered", LogSource.TEXT);
                        return Status.REGISTERED;
                    }
                case "#disconnect":
                    lastDcedClient = GetSocketInfo(client, false);
                    ConsoleLogger.Log("Client " + lastDcedClient + " disconnected from session", LogSource.TEXT);
                    client.Close();
                    return Status.DISCONNECTED;
            }
        }

        public static string GetSocketInfo(TcpClient client, bool bFull)
        {
            IPEndPoint clientEndPoint = (IPEndPoint)client.Client.LocalEndPoint;
            string clientSocket = clientEndPoint.Address.ToString() + ":" + clientEndPoint.Port.ToString();
            string socketInfo = "client endpoint: " + clientSocket;
            if (bFull)
            {
                socketInfo += "\n";
                IPEndPoint serverEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
                string serverSocket = serverEndPoint.Address.ToString() + ":" + serverEndPoint.Port.ToString();
                socketInfo += "server endpoint: " + serverSocket + "\n";
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
            client.GetStream().Read(loginBuf, 0, bufSize);
            string passwordMsg = "password: ";
            byte[] passwordByte = System.Text.Encoding.Unicode.GetBytes(passwordMsg);
            foreach (byte b in passwordByte) client.GetStream().WriteByte(b);
            client.GetStream().Read(passwordBuf, 0, bufSize);
            string login = System.Text.Encoding.Unicode.GetString(loginBuf);
            string password = System.Text.Encoding.Unicode.GetString(passwordBuf);

            return new KeyValuePair<string, string>(login, password);
        }
    }
}
