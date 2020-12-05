using System;
using IO_TCPServer_API;

namespace IO_TCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleLogger.LogLevel = LogLevel.INFO;
            JsonTcpServer server = new JsonTcpServer("127.0.0.1", 8010, 1024);
            DBManager.Connect();
            server.Listen();
            server.AcceptClient();
            server.Stop();
        }
    }
}
