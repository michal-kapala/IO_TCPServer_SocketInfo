using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace IO_TCPServer_API
{
    public class SimpleTCPServer
    {
        TcpListener listener;
        byte[] buffer;
        const int bufferSize = 1024;
        const String help = "#help\tdisplay this list\n" +
                            "#info\tdisplay socket information\n" +
                            "#disconnect\tend TCP session\n";
        public delegate void TransmissionDelegate(TcpClient client);

        public SimpleTCPServer(String ip, ushort port, uint bufferSize)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            buffer = new byte[bufferSize];
        }

        public void SendData(TcpClient client)
        {
            String tip = "Use '#help' for command list\n";
            byte[] tipMsg = System.Text.Encoding.Unicode.GetBytes(tip);
            NetworkStream stream = client.GetStream();
            try
            { 
                foreach (byte b in tipMsg) stream.WriteByte(b);
                int dataSize;
                byte[] reply;
                while ((dataSize = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    String command = System.Text.Encoding.UTF8.GetString(buffer, 0, dataSize);
                    switch (command)
                    {
                        case "help":
                        case "#help":
                            reply = System.Text.Encoding.Unicode.GetBytes(help);
                            stream.Write(reply, 0, reply.Length);
                            Console.WriteLine(DateTime.Now.ToString() + "\tSent help\n");
                            break;
                        case "#info":
                            string socketInfo = GetSocketInfo(client, true);
                            reply = System.Text.Encoding.Unicode.GetBytes(socketInfo);
                            stream.Write(reply, 0, reply.Length);
                            Console.WriteLine(DateTime.Now.ToString() + "\tSent socket info\n");
                            break;
                        case "#disconnect":
                            client.Close();
                            Console.WriteLine(DateTime.Now.ToString() + "\tClient disconnected from session");
                            break;
                        default: break;
                    }
                        if (command == "#disconnect") break;
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(DateTime.Now.ToString() + "\tData transmission exception: " + e.ToString());
            }
        }
        public void TransmissionCallbackStub(IAsyncResult result)
        {
            Console.WriteLine("Client connection has been handled");
        }

        public void Listen()
        {
            listener.Start();
            Console.WriteLine(DateTime.Now.ToString() + "\tServer is running");
        }

        public void AcceptClient()
        {
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine(DateTime.Now.ToString() + "\tNew connection established: " + GetSocketInfo(client, false));
                TransmissionDelegate transDelegate = new TransmissionDelegate(SendData);
                transDelegate.BeginInvoke(client, TransmissionCallbackStub, client);
            }
        }

        public string GetSocketInfo(TcpClient client, bool bFull)
        {
            IPEndPoint clientEndPoint = (IPEndPoint)client.Client.LocalEndPoint;
            string clientSocket = clientEndPoint.Address.ToString() + ":" + clientEndPoint.Port.ToString();
            string socketInfo = "client endpoint: " + clientSocket + "\n";
            if (bFull)
            {
                IPEndPoint serverEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
                string serverSocket = serverEndPoint.Address.ToString() + ":" + serverEndPoint.Port.ToString();
                socketInfo += "server endpoint: " + serverSocket + "\n";
            }
            return socketInfo;
        }

        public void Stop()
        {
            listener.Stop();
        }
    }
}
