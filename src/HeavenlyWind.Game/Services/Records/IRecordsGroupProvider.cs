using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public interface IRecordsGroupProvider
    {
        RecordsGroup Create(SQLiteConnection rpConnection);
    }
}
