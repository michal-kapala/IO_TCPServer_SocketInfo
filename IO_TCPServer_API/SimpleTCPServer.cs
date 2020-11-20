using System;
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
        public delegate void TransmissionDelegate(TcpClient client);

        public SimpleTCPServer(string ip, ushort port, uint bufferSize)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            buffer = new byte[bufferSize];
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
                    TextProtocol.Process(client, command);
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
