using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace IO_TCPServer_API
{
    public class User
    {
        TcpClient tcpClient;
        public TcpClient Client { get =>tcpClient; }
        KeyValuePair<string, string> credentials;
        //for future user functionalities
        public bool SignedIn { get; set; }
        public bool ChatMode { get; set; }
        public string Login { get => credentials.Key; }
        public User(TcpClient client, string login, string pass)
        {
            tcpClient = client;
            credentials = new KeyValuePair<string, string>(login, Helper.MakeSHA256Hash(pass));
            SignedIn = true;
            ChatMode = false;
        }
    }
}
