using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze
{
    public class Logger
    {
        static Logger Instance { get; } = new Logger();

        StreamWriter r_Writer;

        ConcurrentQueue<LogItem> r_Queue;

        Task r_LoggingTask;
        ManualResetEventSlim r_HasNewItems;

        public static event Action<LogItem> LogAdded = delegate { };

        Logger()
        {
            var rCurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var rLogDirectory = new DirectoryInfo(Path.Combine(rCurrentDirectory, "Log"));
            if (!rLogDirectory.Exists)
                rLogDirectory.Create();

            r_Writer = new StreamWriter(Path.Combine(rLogDirectory.FullName, DateTime.Now.ToString("yyyy-MM-dd") + ".log"), true, new UTF8Encoding(true));

            r_Queue = new ConcurrentQueue<LogItem>();

            r_LoggingTask = Task.Factory.StartNew(Process, TaskCreationOptions.LongRunning);
            r_HasNewItems = new ManualResetEventSlim(false);
        }

        void Process()
        {
            while (true)
            {
                r_HasNewItems.Wait();
                r_HasNewItems.Reset();

                Thread.Sleep(100);

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
    }
}
