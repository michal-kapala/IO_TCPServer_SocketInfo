using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;

namespace IO_TCPClient
{
    public static class Helper
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
            for(int i = 0; i < buffer.Length; i++)
            {
                // \r or \n
                if((uint)buffer[i] == 0xA || (uint)buffer[i] == 0xD)
                    buffer[i] = 0;
            }

            int index = 0;
            while((uint)buffer[index] != 0) index++;
            byte[] copy = new byte[index];
            for (int i = 0; i < index; i++) copy[i] = buffer[i];
            return System.Text.Encoding.UTF8.GetString(copy);
        }

        public static string MakeSHA256Hash(string input)
        {
            byte[] data = Encoding.Unicode.GetBytes(input);
            byte[] hash = new byte[32];
            using (SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider())
            {
                hash = sha256.ComputeHash(data);
            }
            string result = Encoding.Unicode.GetString(hash);
            return result;
        }

        public static string ReadIntoJson(TcpClient client)
        {
            while (!client.GetStream().DataAvailable) Thread.Sleep(500);
            byte[] sizeBuffer = new byte[4];
            ReadNetStream(client, sizeBuffer, 0, sizeBuffer.Length);
            int jsonSize = BitConverter.ToInt32(sizeBuffer, 0);
            byte[] jsonBuffer = new byte[jsonSize];
            ReadNetStream(client, jsonBuffer, 0, jsonBuffer.Length);
            return Encoding.UTF8.GetString(jsonBuffer);
        }

        public static byte[] AppendBufferSize(byte[] buffer)
        {
            string concat = buffer.Length.ToString() + Encoding.UTF8.GetString(buffer);
            byte[] result = Encoding.UTF8.GetBytes(concat);
            return result;
        }
    }
}
