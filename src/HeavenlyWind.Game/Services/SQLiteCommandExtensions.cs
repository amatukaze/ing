using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public static class SQLiteCommandExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PostToTransactionQueue(this SQLiteCommand rpCommand) => RecordService.Instance.PostTransaction(rpCommand);
    }
}
