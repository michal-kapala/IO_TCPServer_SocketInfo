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
        Credentials credentials;
        //for future user functionalities
        public bool SignedIn { get; set; }
        public bool ChatMode { get; set; }
        public string Login { get => credentials.Login; }
        public User(TcpClient client, string login, string pass)
        {
            tcpClient = client;
            credentials = new Credentials(login, pass);
            SignedIn = true;
            ChatMode = false;
        }

        public User(TcpClient client, Credentials cr)
        {
            tcpClient = client;
            credentials = cr;
        }
    }
}
