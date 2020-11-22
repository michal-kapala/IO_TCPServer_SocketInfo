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
        public delegate void TransmissionDelegate(TcpClient client);
        List<string> messages;
        Dictionary<int, TcpClient> clients;
        List<string> db;

        public SimpleTCPServer(string ip, ushort port, uint bufferSize)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            messages = new List<string>();
            clients = new Dictionary<int, TcpClient>();
            db = new List<string>();
        }

        private void BroadcastMessages()
        {
            string clear = String.Concat(Enumerable.Repeat("\n", 100));
            byte[] buffer = new byte[1024];
            foreach (KeyValuePair<int, TcpClient> client in clients){
                client.Value.GetStream().Write(System.Text.Encoding.Unicode.GetBytes(clear), 0, clear.Length);
                foreach (string message in messages)
                {
                    buffer = System.Text.Encoding.Unicode.GetBytes(message + "\n");
                    client.Value.GetStream().Write(buffer, 0, buffer.Length);
                }
                buffer = System.Text.Encoding.Unicode.GetBytes("> ");
                client.Value.GetStream().Write(buffer, 0, buffer.Length);
            }
        }

        public void HandleConnection(TcpClient client)
        {
            byte[] buffer = new byte[1024];
            NetworkStream stream = client.GetStream();
            KeyValuePair<string, string> creds;
            Regex sanitize = new Regex("[^a-zA-Z0-9#(+ +)]");
            int dataSize;
            TextProtocol.Status status = TextProtocol.Status.HELP;
            try
            {
                TextProtocol.SendHelpMsg(client);
                while ((dataSize = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string command = System.Text.Encoding.UTF8.GetString(buffer, 0, dataSize);
                    command = sanitize.Replace(command, "");
                    switch (command)
                    {
                        case "#register":
                        case "#signin":
                            creds = TextProtocol.GetCredentials(client);
                            status = TextProtocol.Process(client, command, creds.Key, creds.Value);
                            break;
                        default:
                            status = TextProtocol.Process(client, command, null, null);
                            break;
                    }
                    switch(status)
                    {
                        default: break;
                        case TextProtocol.Status.SIGNED_IN:
                            break;
                        case TextProtocol.Status.DISCONNECTED:
                            break;
                        case TextProtocol.Status.EXCEPTION:
                            break;
                    }
                }
                if(status == TextProtocol.Status.SIGNED_IN)
                {
                    clients.Add(client.GetHashCode(), client);
                }
                else
                {
                    return;
                }
                while((dataSize = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string command = System.Text.Encoding.UTF8.GetString(buffer, 0, dataSize);
                    command = sanitize.Replace(command, "");
                    status = TextProtocol.Process(client, command, null, null);
                    if (status == TextProtocol.Status.DISCONNECTED)
                    {
                        break;
                    }
                    messages.Add(command);
                    BroadcastMessages();
                }
            }
            catch (IOException e)
            {
                ConsoleLogger.Log("Data transmission exception: " + e.ToString(), LogSource.SERVER, LogLevel.ERROR);
            }
        }
        public void TransmissionCallbackStub(IAsyncResult result)
        {
            ConsoleLogger.Log("Client " + TextProtocol.LastDCedClient + " connection has been closed", LogSource.SERVER, LogLevel.INFO);
        }

        public void Listen()
        {
            listener.Start();
            ConsoleLogger.Log("Server is running", LogSource.SERVER, LogLevel.INFO);
        }

        public void AcceptClient()
        {
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                ConsoleLogger.Log("New connection established: " + TextProtocol.GetSocketInfo(client, false), LogSource.SERVER, LogLevel.INFO);
                TransmissionDelegate transDelegate = new TransmissionDelegate(HandleConnection);
                transDelegate.BeginInvoke(client, TransmissionCallbackStub, client);
            }
        }

        public void Stop()
        {
            listener.Stop();
        }
    }
}
