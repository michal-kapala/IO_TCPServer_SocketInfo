using System;
using IO_TCPServer_API;

namespace IO_TCPServer_SocketInfo
{
    class Program
    {
        static void Main(string[] args)
        {
<<<<<<< Updated upstream
=======
            ConsoleLogger.LogLevel = LogLevel.DEBUG;
>>>>>>> Stashed changes
            SimpleTCPServer server = new SimpleTCPServer("127.0.0.1", 8010, 1024);
            server.Listen();
            server.AcceptClient();
            server.Stop();
        }
    }
}
