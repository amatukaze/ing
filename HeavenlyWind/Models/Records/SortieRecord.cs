using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Models.Records
{
    using Game;
    using System.Data.SQLite;
    using EventMapDifficultyEnum = EventMapDifficulty;

    class SortieRecord : ModelBase
    {
        internal long ID { get; }
        public long SortieID { get; }

        public IMapMasterInfo Map { get; }
        public bool IsEventMap { get; }
        public EventMapDifficultyEnum? EventMapDifficulty { get; }

        public int Step { get; }
        public int Node { get; }
        public string NodeWikiID { get; }

        public SortieEventType EventType { get; }
        public BattleType BattleType { get; }

        public string Time { get; }

        public BattleRank? BattleRank { get; private set; }
        public ShipInfo DroppedShip { get; private set; }

        public bool IsBattleDetailAvailable { get; private set; }

        public IList<ShipInfo> HeavilyDamagedShips { get; private set; }

        internal SortieRecord(SQLiteDataReader rpReader)
        {
            SortieID = Convert.ToInt64(rpReader["id"]);

            var rMapID = Convert.ToInt32(rpReader["map"]);
            Map = MapService.Instance.GetMasterInfo(rMapID);

            var rEventMapDifficulty = (EventMapDifficultyEnum)Convert.ToInt32(rpReader["difficulty"]);
            IsEventMap = rEventMapDifficulty != EventMapDifficultyEnum.None;
            if (IsEventMap)
                EventMapDifficulty = rEventMapDifficulty;

            Step = Convert.ToInt32(rpReader["step"]);
            Node = Convert.ToInt32(rpReader["node"]);
            NodeWikiID = MapService.Instance.GetNodeWikiID(rMapID, Node);

            EventType = (SortieEventType)Convert.ToInt32(rpReader["type"]);
            if (EventType == SortieEventType.NormalBattle)
                BattleType = (BattleType)Convert.ToInt32(rpReader["subtype"]);

            if (EventType == SortieEventType.NormalBattle || EventType == SortieEventType.BossBattle)
                Time = DateTimeUtil.FromUnixTime(Convert.ToUInt64(rpReader["extra_info"])).LocalDateTime.ToString();

            ID = Convert.ToInt64(rpReader["extra_info"]);

            Update(rpReader);
        }

        internal void Update(SQLiteDataReader rpReader)
        {
            var rBattleRank = rpReader["rank"];
            if (rBattleRank != DBNull.Value)
            {
                BattleRank = (BattleRank)Convert.ToInt32(rBattleRank);
                OnPropertyChanged(nameof(BattleRank));
            }

            var rDroppedShip = rpReader["dropped_ship"];
            if (rDroppedShip != DBNull.Value)
            {
                DroppedShip = KanColleGame.Current.MasterInfo.Ships[Convert.ToInt32(rDroppedShip)];
                OnPropertyChanged(nameof(DroppedShip));
            }

            IsBattleDetailAvailable = Convert.ToBoolean(rpReader["battle_detail"]);
            OnPropertyChanged(nameof(IsBattleDetailAvailable));

            var rHeavilyDamagedShipIDs = rpReader["heavily_damaged"];
            if (rHeavilyDamagedShipIDs != DBNull.Value)
            {
                HeavilyDamagedShips = ((string)rpReader["heavily_damaged"]).Split(',').Select(r => KanColleGame.Current.MasterInfo.Ships[int.Parse(r)]).ToList();
                OnPropertyChanged(nameof(HeavilyDamagedShips));
            }
        }
    }
}
