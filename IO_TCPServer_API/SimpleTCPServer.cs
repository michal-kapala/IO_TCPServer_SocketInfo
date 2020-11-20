using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace IO_TCPServer_API
{
    public class SimpleTCPServer
    {
        TcpListener listener;
        byte[] buffer;
        const int bufferSize = 1024;
        public delegate void TransmissionDelegate(TcpClient client);
        List<String> messages;
        Dictionary<int, TcpClient> clients;

        public SimpleTCPServer(string ip, ushort port, uint bufferSize)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            buffer = new byte[bufferSize];
            messages = new List<String>();
            clients = new Dictionary<int, TcpClient>();
        }

        private void BroadcastMessage(String message)
        {
            foreach (KeyValuePair<int, TcpClient> client in clients){
                byte[] buffer = System.Text.Encoding.Unicode.GetBytes(message);
                client.Value.GetStream().Write(buffer, 0, buffer.Length);
            }
        }

        public void SendData(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            try
            {
                TextProtocol.SendTipMsg(client);
                int dataSize;
                while ((dataSize = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string command = System.Text.Encoding.UTF8.GetString(buffer, 0, dataSize);
                    Regex sanitize = new Regex("[^a-zA-Z0-9#]");
                    command = sanitize.Replace(command, "");
                    BroadcastMessage(command);
                    if (command == "#disconnect") break;
                }
            }
            catch (IOException e)
            {
                ConsoleLogger.Log("Data transmission exception: " + e.ToString(), LogSource.SERVER);
            }
        }
        public void TransmissionCallbackStub(IAsyncResult result)
        {
            ConsoleLogger.Log("Client " + TextProtocol.LastDCedClient + " connection has been closed", LogSource.SERVER);
        }

        public void Listen()
        {
            listener.Start();
            ConsoleLogger.Log("Server is running", LogSource.SERVER);
        }

        public void AcceptClient()
        {
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                clients.Add(client.GetHashCode(), client);
                ConsoleLogger.Log("New connection established: " + TextProtocol.GetSocketInfo(client, false), LogSource.SERVER);
                TransmissionDelegate transDelegate = new TransmissionDelegate(SendData);
                transDelegate.BeginInvoke(client, TransmissionCallbackStub, client);
            }
        }

        public void Stop()
        {
            listener.Stop();
        }
    }
}
