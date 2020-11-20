using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace IO_TCPServer_API
{
    public class SimpleTCPServer
    {
        TcpListener listener;
        byte[] buffer;
        const int bufferSize = 1024;
        public delegate void TransmissionDelegate(TcpClient client);
        List<string> messages;
        Dictionary<int, TcpClient> clients;

        public SimpleTCPServer(string ip, ushort port, uint bufferSize)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            buffer = new byte[bufferSize];
            messages = new List<string>();
            clients = new Dictionary<int, TcpClient>();
        }

        private void BroadcastMessages()
        {
            string clear = String.Concat(Enumerable.Repeat("\n", 100));
            foreach (KeyValuePair<int, TcpClient> client in clients){
                client.Value.GetStream().Write(System.Text.Encoding.Unicode.GetBytes(clear), 0, clear.Length);
                foreach (string message in messages)
                {
                    byte[] buffer = System.Text.Encoding.Unicode.GetBytes(message + "\n");
                    client.Value.GetStream().Write(buffer, 0, buffer.Length);
                }
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
                    Regex sanitize = new Regex("[^a-zA-Z0-9#(+ +)]");
                    command = sanitize.Replace(command, "");
                    messages.Add(command);
                    BroadcastMessages();
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
