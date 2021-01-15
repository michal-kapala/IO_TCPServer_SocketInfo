using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using JsonProtocol;
using System.Text.Json;
using IO_TCPServer_API;

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
            while(!tcpClient.Connected)
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

        public bool SendRequest(object content, RequestId reqId)
        {
            byte[] bytes;
            Credentials cr;
            switch(reqId)
            {
                default: return false;
                case RequestId.SignIn:
                    cr = (Credentials)content;
                    Credentials credentials = new Credentials(cr.Login, cr.Password);
                    SignInRequest signIn = new SignInRequest(credentials);
                    bytes = signIn.Json;
                    break;
                case RequestId.SignUp:
                    cr = (Credentials)content;
                    Credentials cred = new Credentials(cr.Login, cr.Password);
                    SignUpRequest signUp = new SignUpRequest(cred);
                    bytes = signUp.Json;
                    break;
                case RequestId.ChatMessage:
                    ChatMessage chatMsg = (ChatMessage)content;
                    ChatMessageRequest msgReq = new ChatMessageRequest(chatMsg);
                    bytes = msgReq.Json;
                    break;
            }
            try
            {
                tcpClient.GetStream().Write(bytes, 0, bytes.Length);
                return true;
            }
            catch { return false; }
        }

        public bool RequestSignIn(string login, string pass)
        {
            Credentials cr = new Credentials(login, pass);
            return SendRequest(cr, RequestId.SignIn);
        }

        public bool RequestSignUp(string login, string pass)
        {
            Credentials cr = new Credentials(login, pass);
            return SendRequest(cr, RequestId.SignUp);
        }

        public bool RequestChatMessage(string user, string msg)
        {
            ChatMessage chatMsg = new ChatMessage(user, msg, DateTime.Now.Millisecond.ToString());
            return SendRequest(chatMsg, RequestId.ChatMessage);
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
