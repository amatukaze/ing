using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Models.Records
{
    class SortieRecord : SortieRecordBase, IRecordID
    {
        public long ID { get; }
        public long SortieID { get; }

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

        public ShipInfo MvpShip { get; private set; }
        public ShipInfo EscortFleetMvpShip { get; private set; }

        internal SortieRecord(SQLiteDataReader rpReader) : base(rpReader)
        {
            SortieID = rpReader.GetInt64("id");

            Step = rpReader.GetInt32("step");
            Node = rpReader.GetInt32("node");
            NodeWikiID = MapService.Instance.GetNodeWikiID(Map.ID, Node);

            EventType = (SortieEventType)rpReader.GetInt32("type");
            if (EventType == SortieEventType.NormalBattle)
                BattleType = (BattleType)rpReader.GetInt32("subtype");

            if (EventType == SortieEventType.NormalBattle || EventType == SortieEventType.BossBattle)
                Time = DateTimeUtil.FromUnixTime(rpReader.GetInt64("extra_info")).LocalDateTime.ToString();

            ID = rpReader.GetInt64("extra_info");

            Update(rpReader);
        }

        internal void Update(SQLiteDataReader rpReader)
        {
            var rBattleRank = rpReader.GetInt32Optional("rank");
            if (rBattleRank.HasValue)
                BattleRank = (BattleRank)rBattleRank.Value;

            var rDroppedShip = rpReader.GetInt32Optional("dropped_ship");
            if (rDroppedShip.HasValue)
                DroppedShip = KanColleGame.Current.MasterInfo.Ships[rDroppedShip.Value];

            IsBattleDetailAvailable = rpReader.GetBoolean("battle_detail");

            var rHeavilyDamagedShipIDs = rpReader.GetString("heavily_damaged");
            if (rHeavilyDamagedShipIDs != null)
                HeavilyDamagedShips = rHeavilyDamagedShipIDs.Split(',').Select(r => KanColleGame.Current.MasterInfo.Ships[int.Parse(r)]).ToList();

            var rMvpShip = rpReader.GetInt32Optional("mvp");
            if (rMvpShip.HasValue)
                MvpShip = KanColleGame.Current.MasterInfo.Ships[rMvpShip.Value];

            var rEscortFleetMvpShip = rpReader.GetInt32Optional("mvp_escort");
            if (rEscortFleetMvpShip.HasValue)
                EscortFleetMvpShip = KanColleGame.Current.MasterInfo.Ships[rEscortFleetMvpShip.Value];

            OnPropertyChanged(string.Empty);
        }
    }
}
