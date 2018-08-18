using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class FleetManager : ModelBase, IEnumerable<Fleet>
    {
        public IList<Fleet> Items { get; private set; } = new Fleet[0];

        public CombinedFleetType CombinedFleetType { get; internal set; }

        public Fleet this[int rpID] => Items[rpID - 1];

        public event Action<IEnumerable<Fleet>> FleetsUpdated = delegate { };

        internal FleetManager()
        {
            ApiService.Subscribe("api_req_hensei/change", r =>
            {
                var rFleet = this[int.Parse(r.Parameters["api_id"])];

                var rShipID = int.Parse(r.Parameters["api_ship_id"]);
                if (rShipID == -2)
                {
                    rFleet.RemoveAllExceptFlagship();
                    rFleet.Update();
                    return;
                }

                var rIndex = int.Parse(r.Parameters["api_ship_idx"]);
                if (rShipID == -1)
                {
                    rFleet.Remove(rIndex);
                    rFleet.Update();
                    return;
                }

                var rShip = KanColleGame.Current.Port.Ships[rShipID];
                var rOriginalFleet = rShip.OwnerFleet;
                var rOriginalIndex = rOriginalFleet?.Ships.IndexOf(rShip);
                if (rOriginalFleet == rFleet)
                    rOriginalFleet.Swap(rIndex, rOriginalIndex.Value);
                else
                {
                    var rOriginalShip = rFleet.Organize(rIndex, rShip);
                    if (rOriginalIndex.HasValue)
                        rOriginalFleet.Organize(rOriginalIndex.Value, rOriginalShip);
                }

                if ((rFleet.State & FleetState.AnchorageRepair) == FleetState.AnchorageRepair)
                    rFleet.AnchorageRepair.Reset();

                rFleet.Update();
                rOriginalFleet?.Update();
            });

            ApiService.Subscribe("api_get_member/deck", r => Update(r.GetData<RawFleet[]>()));
            ApiService.Subscribe("api_req_hensei/preset_select", r =>
            {
                var rFleet = this[int.Parse(r.Parameters["api_deck_id"])];

                rFleet.Update(r.GetData<RawFleet>());
            });

            ApiService.Subscribe("api_req_map/start", _ => Update());
        }

        internal void Update()
        {
            foreach (var rFleet in Items)
                rFleet.Update();
        }
        internal void Update(RawPort rpPort)
        {
            CombinedFleetType = rpPort.CombinedFleetType;

            Update(rpPort.Fleets);
        }
        internal void Update(RawFleet[] rpFleets)
        {
            if (Items?.Count == rpFleets.Length)
                foreach (var rFleet in rpFleets)
                    this[rFleet.ID].Update(rFleet);
            else
            {
                Items = rpFleets.Select(r => new Fleet(KanColleGame.Current.Port, r)).ToArray();
                FleetsUpdated(Items);
            }
        }

        public IEnumerator<Fleet> GetEnumerator()
        {
            if (Items == null)
                throw new InvalidOperationException();

            return Items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
