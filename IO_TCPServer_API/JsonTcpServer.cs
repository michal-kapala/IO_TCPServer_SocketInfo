﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using JsonProtocol;

namespace IO_TCPServer_API
{
    public class JsonTcpServer : BaseServer
    {
        new private List<ChatMessage> messages;

        public JsonTcpServer(string ip, ushort port, uint bufferSize) : base(ip, port, bufferSize)
        {
            messages = new List<ChatMessage>();
        }

        public override void HandleConnection(TcpClient client)
        {
            byte[] buffer = new byte[1024];
            NetworkStream stream = client.GetStream();
            Regex sanitize = new Regex("[^a-zA-Z0-9#(+ +)]");
            JsonMessage response;
            byte[] responseBytes;
            int dataSize;
            try
            {
                while ((dataSize = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string m = System.Text.Encoding.UTF8.GetString(buffer, 0, dataSize);
                    Console.WriteLine(m);
                    JsonMessage request = JsonSerializer.Deserialize<JsonMessage>(m);
                    switch (request.type)
                    {
                        case "register":
                            response = cJsonProtocol.register(request);
                            responseBytes = JsonSerializer.SerializeToUtf8Bytes(response);
                            stream.Write(responseBytes, 0, responseBytes.Length);
                            break;
                        case "signin":
                            response = cJsonProtocol.signin(request);
                            if(response.status == JsonMessageStatus.Ok){
                                try
                                {
                                    userManager.SignIn(client, request.username, request.password);
                                }
                                catch (Exception ex)
                                {
                                    ConsoleLogger.Log("Sign-in exception:\n" + ex.ToString(), LogSource.TEXT, LogLevel.ERROR);
                                }
                            }
                            responseBytes = JsonSerializer.SerializeToUtf8Bytes(response);
                            stream.Write(responseBytes, 0, responseBytes.Length);
                            break;
                        case "disconnect":
                            response = cJsonProtocol.disconnect(request);
                            responseBytes = JsonSerializer.SerializeToUtf8Bytes(response);
                            stream.Write(responseBytes, 0, responseBytes.Length);
                            break;
                        case "message":
                            response = cJsonProtocol.message(request);
                            responseBytes = JsonSerializer.SerializeToUtf8Bytes(response);
                            foreach(User user in userManager.activeUsers){
                                user.Client.GetStream().Write(responseBytes, 0, responseBytes.Length);
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleLogger.Log("Data transmission exception: " + e.ToString(), LogSource.SERVER, LogLevel.ERROR);
            }
        }
    }
}
