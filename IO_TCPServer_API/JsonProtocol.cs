using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IO_TCPServer_API
{
    class JsonProtocol
    {
        public static JsonMessage disconnect(JsonMessage request)
        {
            JsonMessage response = new JsonMessage(request, JsonMessageStatus.Ok);
            return response;
        }

        public static JsonMessage message(JsonMessage request)
        {
            JsonMessage response = new JsonMessage(request, JsonMessageStatus.Ok);
            return response;
        }

        public static JsonMessage register(JsonMessage request)
        {
            JsonMessage response;
            if (DBManager.AddUser(request.username, request.password))
            {
                ConsoleLogger.Log("User " + request.username + " registered", LogSource.TEXT, LogLevel.INFO);
                 response = new JsonMessage(request, JsonMessageStatus.Ok);
            }
            else
            {
                ConsoleLogger.Log("User " + request.username + " already exists", LogSource.TEXT, LogLevel.ERROR);
                response = new JsonMessage(request, JsonMessageStatus.Err);
            }
            return response;
        }

        public static JsonMessage signin(JsonMessage request)
        {
            JsonMessage response;
            if (DBManager.ValidateUser(request.username, request.password))
            {
                ConsoleLogger.Log("User " + request.username + " signed in.", LogSource.TEXT, LogLevel.INFO);
                response = new JsonMessage(request, JsonMessageStatus.Ok);
            }
            else
            {
                response = new JsonMessage(request, JsonMessageStatus.Err);
            }
            return response;
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
    }
}
