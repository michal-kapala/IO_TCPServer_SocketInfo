using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
<<<<<<< Updated upstream
=======
using System.Collections.Generic;
using System.Threading;
using System.Text;
using JsonProtocol;
using System.Text.Json;
>>>>>>> Stashed changes

namespace IO_TCPServer_API
{
    public class SimpleTCPServer
    {
<<<<<<< Updated upstream
=======
        public enum Status
        {
            SIGNED_IN,
            WRONG_CREDENTIALS,
            REGISTERED,
            EXISTS,
            HELP,
            DISCONNECTED,
            MESSAGE,
            EXCEPTION,
            CHAT_JOIN,
            CHAT_LEAVE,
            ALREADY_SIGNED_IN,
            NO_SUCH_USER
        };

>>>>>>> Stashed changes
        TcpListener listener;
        byte[] buffer;
        const int bufferSize = 1024;
        public delegate void TransmissionDelegate(TcpClient client);
<<<<<<< Updated upstream
=======
        UserManager userManager;
        public UserManager UserManager { get => userManager; }
        List<User> activeUsers;
        public List<string> messages;
        public List<User> Users { get; }
>>>>>>> Stashed changes

        public SimpleTCPServer(string ip, ushort port, uint bufferSize)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            buffer = new byte[bufferSize];
        }

        public void SendData(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
<<<<<<< Updated upstream
            try
            {
                TextProtocol.SendTipMsg(client);
                int dataSize;
                while ((dataSize = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string command = System.Text.Encoding.UTF8.GetString(buffer, 0, dataSize);
                    Regex sanitize = new Regex("[^a-zA-Z0-9#]");
                    command = sanitize.Replace(command, "");
                    TextProtocol.Process(client, command);
                    if (command == "#disconnect") break;
                }
=======
            bool bFirstCmd = true;
            byte[] protocol = null;

            try
            {
                if (stream.DataAvailable)
                {
                    protocol = new byte[2];
                    Helper.ReadNetStream(client, protocol, 0, protocol.Length);
                }

                //text protocol
                if (protocol == null)
                {
                    //TEXT
                    ConsoleLogger.Log(TextProtocol.GetSocketInfo(client, false) + " uses raw text protocol", LogSource.SERVER, LogLevel.DEBUG);
                    byte[] startup = Encoding.UTF8.GetBytes("#h");

                WAKE_TEXT:
                    while (client != null)
                    {
                        if (bFirstCmd)
                        {
                            ProcessTextProtocol(client, startup);
                            bFirstCmd = false;
                        }
                        else
                        {
                            int dataSize = 0;
                            byte[] buffer = new byte[1024];
                            //nothing changed, wait for data
                            if (!stream.DataAvailable) break;
                            dataSize = Helper.ReadNetStream(client, buffer, 0, buffer.Length);
                            byte[] cmdBuffer = new byte[dataSize];
                            for (int i = 0; i < dataSize; i++) cmdBuffer[i] = buffer[i];
                            ProcessTextProtocol(client, cmdBuffer);
                        }
                    }
                    while (!client.GetStream().DataAvailable) Thread.Sleep(500);
                    goto WAKE_TEXT;
                }
                //binary protocol
                else
                {
                    switch (protocol[0])
                    {
                        case 0x1F:
                            switch (protocol[1])
                            {
                                //JSON protocol header {0x1F, 0x1}
                                case 0x1:
                                    //JSON
                                    ConsoleLogger.Log(TextProtocol.GetSocketInfo(client, false) + " uses JSON protocol v1", LogSource.SERVER, LogLevel.DEBUG);
                                    break;
                                default:
                                    ConsoleLogger.Log("Unknown JSON protocol version: " + protocol[1].ToString("X2"), LogSource.SERVER, LogLevel.ERROR);
                                    break;
                            }
                            break;
                        default:
                            ConsoleLogger.Log("Unknown protocol: " + protocol[0].ToString("X2"), LogSource.SERVER, LogLevel.ERROR);
                            break;
                    }
                }
            WAKE_BINARY:
                while (client != null)
                {
                    int dataSize = 0;
                    byte[] buffer = new byte[1024];

                    //wait for request
                    while (!stream.DataAvailable) Thread.Sleep(500);

                    //read request from stream
                    dataSize = Helper.ReadNetStream(client, buffer, 0, buffer.Length);
                    byte[] cmdBuffer = new byte[dataSize];
                    for (int i = 0; i < dataSize; i++) cmdBuffer[i] = buffer[i];

                    HandleJsonRequest(client, Encoding.UTF8.GetString(cmdBuffer));   
                }
                while (!client.GetStream().DataAvailable) Thread.Sleep(1000);
                goto WAKE_BINARY;
>>>>>>> Stashed changes
            }
            catch (IOException e)
            {
                ConsoleLogger.Log("Data transmission exception: " + e.ToString(), LogSource.SERVER);
            }
        }

        /// <summary>
        /// Processes JSON request and sends response.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="jsonRequest"></param>
        public void HandleJsonRequest(TcpClient client, string jsonRequest)
        {
            ConsoleLogger.Log("Server received a JSON request:\n" + jsonRequest, LogSource.SERVER, LogLevel.DEBUG);
            byte[] response = GetJsonResponseForRequest(client, jsonRequest);
            string resp = Encoding.UTF8.GetString(response);
            ConsoleLogger.Log("Server sends JSON response:\n" + resp, LogSource.SERVER, LogLevel.DEBUG);
            client.GetStream().Write(response, 0, response.Length);
        }

        public byte[] GetJsonResponseForRequest(TcpClient client, string jsonRequest)
        {
            byte[] response = null;
            Status status = ProcessJsonRequest(client, jsonRequest);
            switch(status)
            {
                //Sign up
                case Status.REGISTERED:
                    SignUpResponse signedUp = new SignUpResponse(
                        SignUpResponse.StatusId.SignedUp,
                        "You have been registered successfully!");
                    response = signedUp.Json;
                    break;
                case Status.EXISTS:
                    SignUpResponse alreadyExists = new SignUpResponse(
                        SignUpResponse.StatusId.AlreadyExists,
                        "You have been registered successfully!");
                    response = alreadyExists.Json;
                    break;
                //Sign in
                case Status.SIGNED_IN:
                    SignInResponse signedIn = new SignInResponse(
                        SignInResponse.StatusId.SignedIn,
                        "You have been logged in.");
                    response = signedIn.Json;
                    break;
                case Status.ALREADY_SIGNED_IN:
                    SignInResponse alreadyLogged = new SignInResponse(
                        SignInResponse.StatusId.AlreadyLoggedIn,
                        "You are already logged onto a different client!");
                    response = alreadyLogged.Json;
                    break;
                case Status.WRONG_CREDENTIALS:
                    SignInResponse signInFailed = new SignInResponse(
                        SignInResponse.StatusId.WrongCredentials,
                        "Wrong login or password.");
                    response = signInFailed.Json;
                    break;
                case Status.NO_SUCH_USER:
                    SignInRequest req = JsonSerializer.Deserialize<SignInRequest>(jsonRequest);
                    SignInResponse userNotFound = new SignInResponse(
                        SignInResponse.StatusId.NotFound,
                        $"User { req.Credentials.Login } hasnt been registered yet.");
                    break;
                //Message
                case Status.MESSAGE:
                    //TODO
                    break;
                default: break;
            }
            if(response != null) response = Helper.AppendBufferSize(response);
            return response;
        }

        public Status ProcessJsonRequest(TcpClient client, string json)
        {
            int index = json.IndexOf("\"Id\":") + 5;
            string sub = json.Substring(index);
            int id = int.Parse(sub.TrimEnd('}'));
            switch((RequestId)id)
            {
                case RequestId.SignUp:
                    SignUpRequest signUp = JsonSerializer.Deserialize<SignUpRequest>(json);
                    if (DBManager.AddUser(signUp.Credentials.Login, signUp.Credentials.Password, false))
                    {
                        ConsoleLogger.Log($"User { signUp.Credentials.Login } has been registered", LogSource.DB, LogLevel.INFO);
                        return Status.REGISTERED;
                    }
                    else
                    {
                        ConsoleLogger.Log($"User { signUp.Credentials.Login } failed to sign up - username already registered", LogSource.DB, LogLevel.INFO);
                        return Status.EXISTS;
                    }
                case RequestId.SignIn:
                    try
                    {
                        SignInRequest signIn = JsonSerializer.Deserialize<SignInRequest>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (!DBManager.FindUser(signIn.Credentials.Login))
                        {
                            ConsoleLogger.Log($"User { signIn.Credentials.Login } was not found", LogSource.DB, LogLevel.INFO);
                            return Status.NO_SUCH_USER;
                        }
                        if (UserManager.GetUser(signIn.Credentials.Login) != null)
                        {
                            ConsoleLogger.Log($"User { signIn.Credentials.Login } is already logged onto different client!", LogSource.SERVER, LogLevel.INFO);
                            return Status.ALREADY_SIGNED_IN;
                        }
                        if (DBManager.ValidateUser(signIn.Credentials.Login, signIn.Credentials.Password))
                        {
                            UserManager.SignIn(client, signIn.Credentials.Login, signIn.Credentials.Password);
                            ConsoleLogger.Log($"User { signIn.Credentials.Login } logged in", LogSource.DB, LogLevel.INFO);
                            return Status.SIGNED_IN;
                        }
                        else
                        {
                            ConsoleLogger.Log($"User { signIn.Credentials.Login } failed to log in - wrong password", LogSource.DB, LogLevel.DEBUG);
                            return Status.WRONG_CREDENTIALS;
                        }
                    }
                    catch(Exception ex)
                    {
                        ConsoleLogger.Log("JSON deserialization exception:\n" + ex.ToString(), LogSource.SERVER, LogLevel.ERROR);
                        return Status.EXCEPTION;
                    }
                default:
                    ConsoleLogger.Log($"Unknown JSON request: {id}", LogSource.SERVER, LogLevel.ERROR);
                    return Status.EXCEPTION;
            }
        }

        Status ProcessTextProtocol(TcpClient client, byte[] cmdBuffer)
        { 
            KeyValuePair<string, string> creds;
            Regex sanitize = new Regex("[^a-zA-Z0-9#(+ +)]");

            Status status = Status.HELP;
            string command = Helper.MakeString(cmdBuffer);
            command = sanitize.Replace(command, "");

            if( client != null 
                && status != Status.DISCONNECTED 
                && status != Status.EXCEPTION)
            {
                int dataSize = 0;
                command = sanitize.Replace(command, "");
                
                switch (command)
                {
                    case "#h":
                        status = TextProtocol.ProccessCommand(this, client, command, null, null);
                        break;
                    case "#r":
                        creds = UserManager.GetCredentials(client);
                        status = TextProtocol.ProccessCommand(this, client, command, creds.Key, creds.Value);
                        break;
                    case "#s":
                        creds = UserManager.GetCredentials(client);
                        status = TextProtocol.ProccessCommand(this, client, command, creds.Key, creds.Value);
                        if (status == Status.SIGNED_IN)
                            activeUsers.Add(new User(client, new Credentials(creds.Key, creds.Value)));
                        break;
                    case "#c":
                        User user = UserManager.GetUser(client);
                        status = TextProtocol.ProccessCommand(this, client, command, user.Login, null);
                        //chat loop
                        if (status == Status.CHAT_JOIN)
                        {
                            byte[] buffer = new byte[1024];
                            while ((dataSize = client.GetStream().Read(buffer, 0, buffer.Length)) != 0)
                            {
                                string msg = System.Text.Encoding.UTF8.GetString(buffer, 0, dataSize);
                                msg = sanitize.Replace(msg, "");
                                if (msg == "#c")
                                    status = TextProtocol.ProccessCommand(this, client, msg, user.Login, null);
                                if (status == Status.CHAT_LEAVE)
                                    break;
                                if (status == Status.CHAT_JOIN)
                                {
                                    messages.Add(user.Login + ": " + msg);
                                    TextProtocol.BroadcastMessages(this);
                                }
                                while (!client.GetStream().DataAvailable && client != null) Thread.Sleep(500);
                            }
                        }
                        break;
                    default:
                        status = TextProtocol.ProccessCommand(this, client, command, null, null);
                        break;
                }
                switch (status)
                {
                    default: break;
                    case Status.HELP:
                        break;
                    case Status.SIGNED_IN:
                        break;
                    case Status.DISCONNECTED:
                        client.Close();
                        break;
                    case Status.EXCEPTION:
                        client.Close();
                        break;
                    case Status.WRONG_CREDENTIALS:
                        UserManager.HandleWrongCredentials(this, client, command);
                        break;
                }
            }
            return status;
        }

        public void TransmissionCallbackStub(IAsyncResult result)
        {
            ConsoleLogger.Log(TextProtocol.LastDCedClient + " connection has been closed", LogSource.SERVER);
        }

        public void Listen()
        {
            listener.Start();
            ConsoleLogger.Log("Server is running", LogSource.SERVER);
        }

        public void AcceptClient()
        {
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                ConsoleLogger.Log("New connection established: " + TextProtocol.GetSocketInfo(client, false), LogSource.SERVER);
                TransmissionDelegate transDelegate = new TransmissionDelegate(SendData);
                transDelegate.BeginInvoke(client, TransmissionCallbackStub, client);
            }
        }

        public void Stop()
        {
            listener.Stop();
        }
    }
}
