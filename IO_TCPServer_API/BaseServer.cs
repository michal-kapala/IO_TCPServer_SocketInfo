using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;

namespace IO_TCPServer_API
{
    abstract public class BaseServer
    {
        protected TcpListener listener;
        protected delegate void TransmissionDelegate(TcpClient client);
        public UserManager userManager;
        public List<User> activeUsers;
        public List<string> messages;
        public List<User> Users { get; }

        public BaseServer(string ip, ushort port, uint bufferSize)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            userManager = new UserManager();
            activeUsers = new List<User>();
            messages = new List<string>();
        }

        abstract public void HandleConnection(TcpClient client);

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
