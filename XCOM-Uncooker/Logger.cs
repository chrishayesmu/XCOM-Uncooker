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

    public class ProgressBar(string title)
    {
        public string Title { get; set; } = title;

        public string CurrentItem { get; private set; } = "";

        public int NumCompleted { get; private set; } = 0;

        public int NumTotal { get; private set; } = 0;

        public int BufferRow = 0; // used for tracking where to write this bar in the console

        public bool HasEverPrinted = false;

        public ConsoleColor BackgroundColor = ConsoleColor.Black;

        public ConsoleColor ForegroundColor = ConsoleColor.Gray;

        public void Update(string item, int numCompleted, int numTotal)
        {
            CurrentItem = item;
            NumCompleted = numCompleted;
            NumTotal = numTotal;
        }
    }

    public class Logger(string name)
    {
        public static LogLevel MinLevel = LogLevel.Info;

        private static ConcurrentQueue<string> pendingMessages = new ConcurrentQueue<string>();
        private static List<ProgressBar> progressBars = [];

        private readonly string Name = $"[{name}]";

        public static void StartBackgroundThread()
        {
            Console.ResetColor();

            var thread = new Thread( () =>
            {
                while (true)
                {
                    Thread.Sleep(50);

                    int numMessagesWritten = 0;
                    while (pendingMessages.TryDequeue(out string message))
                    {
                        Console.WriteLine(message);
                        numMessagesWritten++;
                    }

                    // TODO: if a log comes in while there's any progress bars, we need to reset to the position of the topmost one,
                    // clear all output after that, and then add our log + redo the progress bars from that point
                    int cursorTop = Console.CursorTop;
                    for (int i = 0; i < progressBars.Count; i++)
                    {
                        progressBars[i].BufferRow = cursorTop + i;
                        PrintProgressBar(progressBars[i]);
                    }
                    Console.CursorTop = cursorTop;
                }
            });

            thread.Start();
        }

        private static void PrintProgressBar(ProgressBar progressBar)
        {
            // Check that the bar isn't pending an update still
            if (progressBar.NumTotal <= 0)
            {
                return;
            }

            if (!progressBar.HasEverPrinted)
            {
                progressBar.HasEverPrinted = true;
                progressBar.BufferRow = Console.CursorTop;
                Console.CursorTop++;
            }

            float percentage = progressBar.NumCompleted / (float) progressBar.NumTotal;
            int numEquals = (int) (25 * percentage);
            int numSpaces = 25 - numEquals;
            string equalsString = "".PadLeft(numEquals, '=');
            string spacesString = "".PadLeft(numSpaces, ' ');
            string percentageString = $"{(int) (100 * percentage)}".PadLeft(3);

            Console.BackgroundColor = progressBar.BackgroundColor;
            Console.ForegroundColor = progressBar.ForegroundColor;

            int cursorTop = Console.CursorTop;
            Console.CursorLeft = 0;
            Console.CursorTop = progressBar.BufferRow;
            Console.Write($"[{equalsString}{spacesString}] {percentageString}% - [{progressBar.NumCompleted}/{progressBar.NumTotal}] - {progressBar.Title}: {progressBar.CurrentItem}");
            Console.WriteLine("                                                                                   "); // just clear ref anything from the last time we printed on this line
            Console.CursorTop = cursorTop;

            Console.ResetColor();
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

        public void DisplayProgressBar(ProgressBar progressBar)
        {
            progressBars.Add(progressBar);
        }

        public void RemoveProgressBar(ProgressBar progressBar)
        {
            // Print it one more time in case it's been updated (e.g. marked complete)
            PrintProgressBar(progressBar);
            progressBars.Remove(progressBar);
        }

        private void Log(string message, string level)
        {
            pendingMessages.Enqueue($"{DateTime.Now}  {Name}  {level}  {message}");
        }
    }
}
