using System.Collections.Generic;
using Newtonsoft.Json;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Json
{
    internal class HomeportJson: IHomeportUpdate
    {
        public MaterialJsonArray api_material;
        public FleetJson[] api_deck_port;
        public AdmiralJson api_basic;
        public RepairingDockJson[] api_ndock;
        public ShipJson[] api_ship;
        IReadOnlyCollection<IRawShip> IHomeportUpdate.Ships => api_ship;
        [JsonProperty("api_combined_flag")]
        public KnownCombinedFleet CombinedFleetType { get; set; }
    }
}
