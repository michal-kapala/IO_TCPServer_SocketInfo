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

        public static void SendTipMsg(TcpClient client)
        {
            byte[] tipMsg = System.Text.Encoding.Unicode.GetBytes(tip);
            foreach (byte b in tipMsg) client.GetStream().WriteByte(b);
        }
    }
}
