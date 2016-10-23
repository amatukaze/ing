using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.Models.Records
{
    class SortieRecordBase : ModelBase
    {
        public IMapMasterInfo Map { get; }
        public bool IsEventMap { get; }
        public EventMapDifficulty? EventMapDifficulty { get; }

        public SortieRecordBase(SQLiteDataReader rpReader)
        {
            var rMapID = rpReader.GetInt32("map");
            Map = MapService.Instance.GetMasterInfo(rMapID);

            var rEventMapDifficulty = rpReader.GetInt32Optional("difficulty");
            if (rEventMapDifficulty.HasValue)
            {
                IsEventMap = true;
                EventMapDifficulty = (EventMapDifficulty)rEventMapDifficulty.Value;
            }
        }
    }
}
