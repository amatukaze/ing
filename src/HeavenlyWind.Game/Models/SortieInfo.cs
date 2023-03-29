using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class SortieInfo : ModelBase
    {
        internal static SortieInfo Current { get; private set; }

        static IDictionary<int, int> _shipStockEquipmentCount;

        public long ID { get; }

        public Fleet Fleet { get; }
        public Fleet EscortFleet { get; }

        public IList<IParticipant> MainShips { get; }
        public IList<IParticipant> EscortShips { get; }

        public MapInfo Map { get; }

        public SortieNodeInfo PreviousNode { get; private set; }
        public SortieNodeInfo Node { get; private set; }

        public double? DirectionAngle { get; private set; }

        int r_PendingShipCount;
        public int PendingShipCount
        {
            get { return r_PendingShipCount; }
            private set
            {
                if (r_PendingShipCount != value)
                {
                    r_PendingShipCount = value;
                    OnPropertyChanged(nameof(PendingShipCount));
                }
            }
        }
        int _pendingEquipmentCount;
        public int PendingEquipmentCount
        {
            get { return _pendingEquipmentCount; }
            private set
            {
                if (_pendingEquipmentCount != value)
                {
                    _pendingEquipmentCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public AirForceGroup[] AirForceGroups { get; }

        internal int[] LandBaseAerialSupportRequests { get; set; }

        internal long ReturnTime { get; set; }

        static SortieInfo()
        {
            ApiService.Subscribe("api_port/port", _ => Current = null);

            ApiService.Subscribe(new[] { "api_req_sortie/battleresult", "api_req_combined_battle/battleresult" }, r =>
            {
                var rData = (RawBattleResult)r.Data;
                if (rData.DroppedShip != null)
                {
                    Current.PendingShipCount++;

                    if (_shipStockEquipmentCount != null && _shipStockEquipmentCount.TryGetValue(rData.DroppedShip.ID, out var equipmentCount))
                        Current.PendingEquipmentCount += equipmentCount;
                }

                var rFriendParticipantSnapshots = BattleInfo.Current?.CurrentStage?.Friend;
                if (rFriendParticipantSnapshots != null)
                    foreach (var rSnapshot in rFriendParticipantSnapshots)
                        ((FriendShip)rSnapshot.Participant).Ship.HP.Set(rSnapshot.Maximum, rSnapshot.Current);
            });

            if (!DataStore.TryGet("ship_stock_equipment_count", out byte[] content))
                return;

            var reader = new JsonTextReader(new StreamReader(new MemoryStream(content)));

            _shipStockEquipmentCount = JArray.Load(reader).ToDictionary(r => (int)r["id"], r => (int)r["count"]);
        }
        internal SortieInfo() { }
        internal SortieInfo(long rpID, Fleet rpFleet, int rpMapID)
        {
            ID = rpID;

            Current = this;

            Fleet = rpFleet;
            MainShips = Fleet.Ships.Select(r => new FriendShip(r)).ToList<IParticipant>();

            var rPort = KanColleGame.Current.Port;

            if (rPort.Fleets.CombinedFleetType != 0 && rpFleet.ID == 1)
            {
                EscortFleet = rPort.Fleets[2];
                EscortShips = EscortFleet.Ships.Select(r => new FriendShip(r)).ToList<IParticipant>();
            }

            Map = KanColleGame.Current.Maps[rpMapID];

            if (Map.AvailableAirBaseGroupCount > 0 && rPort.AirBase.Table.TryGetValue(Map.MasterInfo.AreaID, out var groupTable))
            {
                var rAllGroups = groupTable.Values;

                AirForceGroups = rAllGroups.Take(Map.AvailableAirBaseGroupCount).Where(r => r.Option == AirForceGroupOption.Sortie)
                    .Concat(rAllGroups.Where(r => r.Option == AirForceGroupOption.AirDefense))
                    .ToArray();
            }
        }

        internal void Explore(long rpTimestamp, RawMapExploration rpData)
        {
            PreviousNode = Node;
            if (PreviousNode != null)
                PreviousNode.Event = null;

            DirectionAngle = MapService.Instance.GetAngle(Map.ID, rpData.StartNode ?? Node?.ID ?? 0, rpData.Node);
            OnPropertyChanged(nameof(DirectionAngle));

            Node = new SortieNodeInfo(this, rpTimestamp, rpData);
            OnPropertyChanged(nameof(Node));
            OnPropertyChanged(nameof(PreviousNode));
        }
    }
}
