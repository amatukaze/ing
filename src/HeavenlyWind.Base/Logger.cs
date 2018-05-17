using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze
{
    public class Logger
    {
        static DirectoryInfo r_ExceptionLogDirectory;
        static Tuple<string, Regex> r_ExceptionLogFilenameRegex;

        static StreamWriter r_Writer;

        static BlockingCollection<LogItem> r_Queue;
        static Task r_LoggingTask;

        public static event Action<LogItem> LogAdded = delegate { };

        static Logger()
        {
            var rCurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var rLogDirectory = new DirectoryInfo(Path.Combine(rCurrentDirectory, "Logs"));
            if (!rLogDirectory.Exists)
                rLogDirectory.Create();

            r_ExceptionLogDirectory = new DirectoryInfo(Path.Combine(rCurrentDirectory, "Logs", "Exceptions"));
            if (!r_ExceptionLogDirectory.Exists)
                r_ExceptionLogDirectory.Create();

            var rStream = File.Open(Path.Combine(rLogDirectory.FullName, DateTime.Now.ToString("yyyy-MM-dd") + ".log"), FileMode.Append, FileAccess.Write, FileShare.Write);
            r_Writer = new StreamWriter(rStream, new UTF8Encoding(true));

            r_Queue = new BlockingCollection<LogItem>();
            r_LoggingTask = Task.Factory.StartNew(Process, TaskCreationOptions.LongRunning);
        }

        static void Process()
        {
            foreach (var rItem in r_Queue.GetConsumingEnumerable())
            {
                LogAdded(rItem);

                r_Writer.WriteLine(rItem);
                r_Writer.Flush();
            }
        }

        public static void Write(LoggingLevel rpLevel, string rpContent) => r_Queue.Add(new LogItem(DateTime.Now, rpLevel, rpContent));

        public static string GetNewExceptionLogFilename()
        {
            var rPrefix = DateTime.Now.ToString("yyMMdd");
            Regex rRegex;
            if (r_ExceptionLogFilenameRegex != null && r_ExceptionLogFilenameRegex.Item1 == rPrefix)
                rRegex = r_ExceptionLogFilenameRegex.Item2;
            else
                r_ExceptionLogFilenameRegex = Tuple.Create(rPrefix, rRegex = new Regex(rPrefix + @"_(\d+)\.log$", RegexOptions.IgnoreCase));

            var rIndex = 0;
            var rSavedLogs = r_ExceptionLogDirectory.GetFiles(rPrefix + "_*.log");
            if (rSavedLogs.Length > 0)
                rIndex = rSavedLogs.Max(r => int.Parse(rRegex.Match(r.FullName).Groups[1].Value));

            rIndex++;

            return Path.Combine(r_ExceptionLogDirectory.FullName, rPrefix + "_" + rIndex + ".log");
        }
    }
}
