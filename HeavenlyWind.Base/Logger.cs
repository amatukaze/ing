using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze
{
    public class Logger
    {
        static Tuple<string, Regex> r_ExceptionLogFilenameRegex;

        static Logger Instance { get; } = new Logger();

        StreamWriter r_Writer;

        ConcurrentQueue<LogItem> r_Queue;

        Task r_LoggingTask;
        ManualResetEventSlim r_HasNewItems;

        public static event Action<LogItem> LogAdded = delegate { };

        Logger()
        {
            var rCurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var rLogDirectory = new DirectoryInfo(Path.Combine(rCurrentDirectory, "Logs"));
            if (!rLogDirectory.Exists)
                rLogDirectory.Create();

            r_Writer = new StreamWriter(Path.Combine(rLogDirectory.FullName, DateTime.Now.ToString("yyyy-MM-dd") + ".log"), true, new UTF8Encoding(true));

            r_Queue = new ConcurrentQueue<LogItem>();

            r_LoggingTask = Task.Factory.StartNew(Process, TaskCreationOptions.LongRunning);
            r_HasNewItems = new ManualResetEventSlim(false);
        }

        async void Process()
        {
            while (true)
            {
                r_HasNewItems.Wait();
                r_HasNewItems.Reset();

                await Task.Delay(100);

                LogItem rItem;
                while (r_Queue.TryDequeue(out rItem))
                {
                    r_Writer.WriteLine(rItem);
                    LogAdded(rItem);
                }

                r_Writer.Flush();
            }
        }

        void WriteCore(LoggingLevel rpLevel, string rpContent)
        {
            r_Queue.Enqueue(new LogItem(DateTime.Now, rpLevel, rpContent));

            r_HasNewItems.Set();
        }

        public static void Write(LoggingLevel rpLevel, string rpContent) => Task.Run(() => Instance.WriteCore(rpLevel, rpContent));

        public static string GetNewExceptionLogFilename()
        {
            var rCurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var rExceptionLogDirectory = new DirectoryInfo(Path.Combine(rCurrentDirectory, "Logs", "Exceptions"));
            if (!rExceptionLogDirectory.Exists)
                rExceptionLogDirectory.Create();

            var rPrefix = DateTime.Now.ToString("yyMMdd");
            Regex rRegex;
            if (r_ExceptionLogFilenameRegex != null && r_ExceptionLogFilenameRegex.Item1 == rPrefix)
                rRegex = r_ExceptionLogFilenameRegex.Item2;
            else
                r_ExceptionLogFilenameRegex = Tuple.Create(rPrefix, rRegex = new Regex(rPrefix + @"_(\d+)\.log$", RegexOptions.IgnoreCase));

            var rIndex = 0;
            var rSavedLogs = rExceptionLogDirectory.GetFiles(rPrefix + "_*.log");
            if (rSavedLogs.Any())
                rIndex = rSavedLogs.Max(r => int.Parse(rRegex.Match(r.FullName).Groups[1].Value));

            return Path.Combine(rExceptionLogDirectory.FullName, $"{rPrefix}_{rIndex + 1}.log");
        }
    }
}
