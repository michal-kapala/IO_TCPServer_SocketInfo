﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace IO_TCPServer_API
{
    static class Helper
    {
        //since NetworkStream.Read() doesnt wait for user input
        public static int ReadNetStream(TcpClient client, byte[] buffer, int offset, int bufSize)
        {
            NetworkStream stream = client.GetStream();
            int index = 0;
            if (stream.DataAvailable)
            {
                //skip bytes
                for (int i = 0; i < offset; i++) stream.ReadByte();
                //read
                do
                {
                    buffer[index] = (byte)stream.ReadByte();
                    index++;
                }
                while (stream.DataAvailable && index < bufSize);
            }
            return index;
        }

        public static string MakeString(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                // \r or \n
                if ((uint)buffer[i] == 0xA || (uint)buffer[i] == 0xD)
                    buffer[i] = 0;
            }

            int index = 0;
            while (index < buffer.Length)
            {
                if ((uint)buffer[index] == 0) break;
                index++;
            }
            byte[] copy = new byte[index];
            for (int i = 0; i < index; i++) copy[i] = buffer[i];
            return System.Text.Encoding.UTF8.GetString(copy);
        }

        public static string MakeSHA256Hash(string input)
        {
            byte[] data = Encoding.UTF8.GetBytes(input);
            byte[] hash = new byte[32];
            using (SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider())
            {
                hash = sha256.ComputeHash(data);
            }
            string result = Encoding.UTF8.GetString(hash);
            return result;
        }

        public static string ReadIntoJson(TcpClient client)
        {
            byte[] sizeBuffer = new byte[4];
            Helper.ReadNetStream(client, sizeBuffer, 0, sizeBuffer.Length);
            if (BitConverter.IsLittleEndian) Array.Reverse(sizeBuffer);
            int jsonSize = BitConverter.ToInt32(sizeBuffer, 0);
            byte[] jsonBuffer = new byte[jsonSize];
            Helper.ReadNetStream(client, jsonBuffer, 0, jsonBuffer.Length);
            return Encoding.UTF8.GetString(jsonBuffer);
        }

        public static byte[] AppendBufferSize(byte[] buffer)
        {
            byte[] size = BitConverter.GetBytes(buffer.Length);
            byte[] result = new byte[buffer.Length + size.Length];
            int index = 0;
            foreach (byte b in size)
            {
                result[index] = b;
                index++;
            }
            foreach (byte b in buffer)
            {
                result[index] = b;
                index++;
            }

            return result;
        }
    }
}