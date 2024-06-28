using Konsole;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCOM_Uncooker
{
    public enum LogLevel
    {
        Verbose,
        Info,
        Warning,
        Error,
        System
    }

    public class Logger(string name)
    {
        public static LogLevel MinLevel = LogLevel.Info;

        private readonly static ConcurrentWriter Writer = new ConcurrentWriter();

        private readonly string Name = $"[{name}]";

        public void EmptyLine()
        {
            Writer.WriteLine("");
        }

        public void Error(string message)
        {
            if (MinLevel <= LogLevel.Error)
            {
                Log(message, "[ERROR]");
            }
        }

        public void Info(string message)
        {
            if (MinLevel <= LogLevel.Info)
            {
                Log(message, "[INFO]");
            }
        }

        public void System(string message)
        {
            Log(message, "[SYSTEM]");
        }

        public void Verbose(string message)
        {
            if (MinLevel <= LogLevel.Verbose)
            {
                Log(message, "[VERBOSE]");
            }
        }

        public void Warning(string message)
        {
            if (MinLevel <= LogLevel.Warning)
            {
                Log(message, "[WARNING]");
            }
        }

        private void Log(string message, string level)
        {
            Writer.WriteLine($"{DateTime.Now}  {Name}  {level}  {message}");
        }
    }
}
