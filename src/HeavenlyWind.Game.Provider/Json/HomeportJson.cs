using Sakuno.KanColle.Amatsukaze.Game.Models.Knowledge;

namespace Sakuno.KanColle.Amatsukaze.Game.Json
{
    internal class HomeportJson
    {
        public MaterialJson[] api_material;
        public FleetJson[] api_deck_port;
        public AdmiralJson api_basic;
        public RepairingDockJson[] api_ndock;
        public ShipJson[] api_ship;
        public KnownCombinedFleet api_combined_flag;
    }
}
