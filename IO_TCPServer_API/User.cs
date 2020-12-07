using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using JsonProtocol;

namespace IO_TCPServer_API
{
    public class User
    {
        TcpClient tcpClient;
        public TcpClient Client { get =>tcpClient; }
        Credentials Credentials { get; set; }
        //for future user functionalities
        public string Login { get => Credentials.Login; }
        public bool SignedIn { get; set; }
        public bool ChatMode { get; set; }

        public User(TcpClient client, Credentials credentials)
        {
            tcpClient = client;
            Credentials = credentials;
            SignedIn = true;
            ChatMode = false;
        }
    }
}
