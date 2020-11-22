using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO_TCPServer_API
{
    public enum LogSource
    {
        SERVER,
        TEXT,
        DB,
        USER
    }

    public enum LogLevel
    {
        DEBUG,
        INFO,
        ERROR
    }

    public class ConsoleLogger
    {
        public static LogLevel LogLevel { get; set; }

        public static void Log(string text, LogSource src, LogLevel level)
        {
            if (level < LogLevel) return;
            switch (src)
            {
                case LogSource.SERVER:
                    Console.WriteLine(DateTime.Now.ToString() + "\t[" + Enum.GetName(typeof(LogLevel), level) + "]\t[SERVER]\t" + text);
                    break;
                case LogSource.TEXT:
                    Console.WriteLine(DateTime.Now.ToString() + "\t[" + Enum.GetName(typeof(LogLevel), level) + "]\t[TEXT]\t\t" + text);
                    break;
                case LogSource.DB:
                    Console.WriteLine(DateTime.Now.ToString() + "\t[" + Enum.GetName(typeof(LogLevel), level) + "]\t[DB]\t\t" + text);
                    break;
                case LogSource.USER:
                    Console.WriteLine(DateTime.Now.ToString() + "\t[" + Enum.GetName(typeof(LogLevel), level) + "]\t[USER]\t\t" + text);
                    break;
                default: break;
            }
        }
    }
}
