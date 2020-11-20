using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO_TCPServer_API
{
    enum LogSource
    {
        SERVER,
        TEXT
    }

    class ConsoleLogger
    {
        public static void Log(string text, LogSource src)
        {
            switch(src)
            {
                case LogSource.SERVER:
                    Console.WriteLine(DateTime.Now.ToString() + "\t[SERVER]\t" + text);
                    break;
                case LogSource.TEXT:
                    Console.WriteLine(DateTime.Now.ToString() + "\t[TEXT]\t\t" + text);
                    break;
                default: break;
            }
        }
    }
}
