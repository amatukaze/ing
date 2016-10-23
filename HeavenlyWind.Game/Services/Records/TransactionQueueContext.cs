using System.Data.SQLite;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    struct TransactionQueueContext
    {
        public SQLiteCommand Command { get; }

        public TaskCompletionSource<bool> TaskCompletionSource { get; }

        public TransactionQueueContext(SQLiteCommand rpCommand)
        {
            Command = rpCommand;

            TaskCompletionSource = new TaskCompletionSource<bool>();
        }
    }
}
