using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace IO_TCPServer_API
{
    static class Helper
    {
        public static KeyValuePair<TKey, TValue> GetEntry<TKey, TValue>
            (this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return new KeyValuePair<TKey, TValue>(key, dictionary[key]);
        }

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
    }
}
