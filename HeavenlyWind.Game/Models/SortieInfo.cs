using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class SortieInfo : ModelBase
    {
        internal static SortieInfo Current { get; private set; }

        public long ID { get; } = (long)DateTimeOffset.Now.ToUnixTime();

        public Fleet Fleet { get; }
        public Fleet EscortFleet { get; }

        public IList<IParticipant> MainShips { get; }
        public IList<IParticipant> EscortShips { get; }

        public MapInfo Map { get; }

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

        static SortieInfo()
        {
            SessionService.Instance.Subscribe("api_port/port", _ => Current = null);

            SessionService.Instance.Subscribe(new[] { "api_req_sortie/battleresult", "api_req_combined_battle/battleresult" }, r =>
            {
                var rData = (RawBattleResult)r.Data;
                if (rData.DroppedShip != null)
                     Current.PendingShipCount++;
            });
        }
        internal SortieInfo() { }
        internal SortieInfo(Fleet rpFleet, int rpMapID)
        {
            Current = this;

            Fleet = rpFleet;
            MainShips = Fleet.Ships.Select(r => new FriendShip(r)).ToList<IParticipant>().AsReadOnly();

            if (KanColleGame.Current.Port.Fleets.CombinedFleetType != 0 && rpFleet.ID == 1)
            {
                EscortFleet = KanColleGame.Current.Port.Fleets[2];
                EscortShips = EscortFleet.Ships.Select(r => new FriendShip(r)).ToList<IParticipant>().AsReadOnly();
            }

            Map = KanColleGame.Current.Maps[rpMapID];
        }

        void Explore(RawMapExploration rpData)
        {
            DirectionAngle = MapService.Instance.GetAngle(Map.ID, rpData.StartNode ?? Node?.ID ?? 0, rpData.Node);
            OnPropertyChanged(nameof(DirectionAngle));

            Node = new SortieNodeInfo(Map, rpData);

            OnPropertyChanged(nameof(Node));
        }
    }
}
