using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using JsonProtocol;
using System.Text.Json;

namespace IO_TCPClient
{
    public class SimpleTCPClient
    {
        public TcpClient tcpClient;
        ushort LocalPort { get; set; }
        bool ProtocolAnnounced { get; set; }

        public SimpleTCPClient( ushort clientPort)
        {
            LocalPort = clientPort;
            ProtocolAnnounced = false;
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEP = new IPEndPoint(ipAddress, clientPort);
            tcpClient = new TcpClient(localEP);
        }

        public bool Connect(string serverAddress, ushort serverPort)
        {
            if (!tcpClient.Connected)
            {      
                try
                {
                    tcpClient.Connect(serverAddress, serverPort);
                }
                //port conflict
                catch (SocketException ex)
                {
                    if (LocalPort < 65535) LocalPort++;
                    else
                    {
                        Random r = new Random();
                        LocalPort = (ushort)(r.Next() % 65535 + 3000);
                    }
                    IPAddress ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
                    IPEndPoint ipLocalEndPoint = new IPEndPoint(ipAddress, LocalPort);
                    tcpClient = new TcpClient(ipLocalEndPoint);
                }
            }
            //announce JSON protocol { id, version }
            if (!ProtocolAnnounced)
            {
                byte[] usedProtocol = { 0x1F, 0x1 };
                tcpClient.GetStream().Write(usedProtocol, 0, usedProtocol.Length);
                ProtocolAnnounced = true;
            }
            return tcpClient.Connected;
        }

        public bool SendRequest(string login, string password, RequestId reqId)
        {
            byte[] bytes;
            switch(reqId)
            {
                default: return false;
                case RequestId.SignIn:
                    Credentials credentials = new Credentials(login, password);
                    SignInRequest signIn = new SignInRequest(credentials);
                    bytes = signIn.Json;
                    try
                    {
                        tcpClient.GetStream().Write(bytes, 0, bytes.Length);
                        return true;
                    }
                    catch { return false; }
                case RequestId.SignUp:
                    Credentials cred = new Credentials(login, password);
                    SignUpRequest signUp = new SignUpRequest(cred);
                    bytes = signUp.Json;
                    try
                    {
                        tcpClient.GetStream().Write(bytes, 0, bytes.Length);
                        return true;
                    }
                    catch { return false; } 
            }
        }

        public bool RequestSignIn(string login, string pass)
        {
            return SendRequest(login, pass, RequestId.SignIn);
        }

        public bool RequestSignUp(string login, string pass)
        {
            return SendRequest(login, pass, RequestId.SignUp);
        }

        public Tuple<string, bool> ProcessSignInResponse(string jsonObject)
        {
            SignInResponse response = JsonSerializer.Deserialize<SignInResponse>(jsonObject);
            bool bCompleted = response.Status == SignInResponse.StatusId.SignedIn;
            return new Tuple<string, bool>(response.StatusMsg, bCompleted);
        }

        public Tuple<string, bool> ProcessSignUpResponse(string jsonObject)
        {
            SignUpResponse response = JsonSerializer.Deserialize<SignUpResponse>(jsonObject);
            bool bCompleted = response.Status == SignUpResponse.StatusId.SignedUp;
            return new Tuple<string, bool>(response.StatusMsg, bCompleted);
        }
    }
}
