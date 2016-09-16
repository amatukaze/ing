using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class SortieInfo : ModelBase
    {
        internal static SortieInfo Current { get; private set; }

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

        internal int[] LandBaseAerialSupportRequests { get; set; }

        internal long ReturnTime { get; set; }

        static SortieInfo()
        {
            ApiService.Subscribe("api_port/port", _ => Current = null);

            ApiService.Subscribe(new[] { "api_req_sortie/battleresult", "api_req_combined_battle/battleresult" }, r =>
            {
                var rData = (RawBattleResult)r.Data;
                if (rData.DroppedShip != null)
                     Current.PendingShipCount++;

                var rFriendParticipantSnapshots = BattleInfo.Current?.CurrentStage?.Friend;
                if (rFriendParticipantSnapshots != null)
                    foreach (var rSnapshot in rFriendParticipantSnapshots)
                        ((FriendShip)rSnapshot.Participant).Ship.HP.Set(rSnapshot.Maximum, rSnapshot.Current);
            });
        }
        internal SortieInfo() { }
        internal SortieInfo(long rpID, Fleet rpFleet, int rpMapID)
        {
            ID = rpID;

            Current = this;

            Fleet = rpFleet;
            MainShips = Fleet.Ships.Select(r => new FriendShip(r)).ToList<IParticipant>();

            if (KanColleGame.Current.Port.Fleets.CombinedFleetType != 0 && rpFleet.ID == 1)
            {
                EscortFleet = KanColleGame.Current.Port.Fleets[2];
                EscortShips = EscortFleet.Ships.Select(r => new FriendShip(r)).ToList<IParticipant>();
            }

            Map = KanColleGame.Current.Maps[rpMapID];
        }

        internal void Explore(RawMapExploration rpData)
        {
            PreviousNode = Node;
            if (PreviousNode != null)
                PreviousNode.Event = null;

            DirectionAngle = MapService.Instance.GetAngle(Map.ID, rpData.StartNode ?? Node?.ID ?? 0, rpData.Node);
            OnPropertyChanged(nameof(DirectionAngle));

            Node = new SortieNodeInfo(Map, rpData);
            OnPropertyChanged(nameof(Node));
            OnPropertyChanged(nameof(PreviousNode));
        }
    }
}
