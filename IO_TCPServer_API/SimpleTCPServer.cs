using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;

namespace IO_TCPServer_API
{
    public class SimpleTCPServer : BaseServer
    {
        public SimpleTCPServer(string ip, ushort port, uint bufferSize) : base(ip, port, bufferSize)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            userManager = new UserManager();
            activeUsers = new List<User>();
            messages = new List<string>();
        }

        override public void HandleConnection(TcpClient client)
        {
            byte[] buffer = new byte[1024];
            NetworkStream stream = client.GetStream();
            KeyValuePair<string, string> creds;
            Regex sanitize = new Regex("[^a-zA-Z0-9#(+ +)]");
            int dataSize;
            TextProtocol.Status status = TextProtocol.Status.HELP;
            try
            {
                TextProtocol.SendMsgToClient(client, TextProtocol.Help);
                WAKE:
                while ((dataSize = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string command = System.Text.Encoding.UTF8.GetString(buffer, 0, dataSize);
                    command = sanitize.Replace(command, "");
                    switch (command)
                    {
                        case "#register":
                            creds = UserManager.GetCredentials(client);
                            status = TextProtocol.Process(this, client, command, creds.Key, creds.Value);
                            break;
                        case "#signin":
                            creds = UserManager.GetCredentials(client);
                            status = TextProtocol.Process(this, client, command, creds.Key, creds.Value);
                            if (status == TextProtocol.Status.SIGNED_IN)
                                activeUsers.Add(new User(client, creds.Key, creds.Value));
                            break;
                        case "#chat":
                            User user = userManager.GetUser(client);
                            status = TextProtocol.Process(this, client, command, user.Login, null);
                            //chat loop
                            if (status == TextProtocol.Status.CHAT_JOIN)
                            {
                                while ((dataSize = stream.Read(buffer, 0, buffer.Length)) != 0)
                                {
                                    string msg = System.Text.Encoding.UTF8.GetString(buffer, 0, dataSize);
                                    msg = sanitize.Replace(msg, "");
                                    if (msg == "#chat")
                                        status = TextProtocol.Process(this, client, msg, user.Login, null);
                                    if (status == TextProtocol.Status.CHAT_LEAVE)
                                        break;
                                    if (status == TextProtocol.Status.CHAT_JOIN)
                                    {
                                        messages.Add(user.Login + ": " + msg);
                                        TextProtocol.BroadcastMessages(this);
                                    }
                                    while (!client.GetStream().DataAvailable && client != null) Thread.Sleep(500);
                                }
                            }
                            break;
                        default:
                            status = TextProtocol.Process(this, client, command, null, null);
                            break;
                    }
                    switch(status)
                    {
                        default: break;
                        case TextProtocol.Status.SIGNED_IN:
                            break;
                        case TextProtocol.Status.DISCONNECTED:
                            client.Close();
                            return;
                        case TextProtocol.Status.EXCEPTION:
                            client.Close();
                            return;
                        case TextProtocol.Status.WRONG_CREDENTIALS:
                            userManager.HandleWrongCredentials(this, client, command);
                            break;
                    }
                }
                while (!client.GetStream().DataAvailable) Thread.Sleep(1000);
                goto WAKE;
                
            }
            catch (IOException e)
            {
                ConsoleLogger.Log("Data transmission exception: " + e.ToString(), LogSource.SERVER, LogLevel.ERROR);
            }
        }
    }
}
