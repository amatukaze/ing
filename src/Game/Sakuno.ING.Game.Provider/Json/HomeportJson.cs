using System;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
    internal class HomeportJson
    {
        public MaterialJsonArray api_material;
        public FleetJson[] api_deck_port;
        public AdmiralJson api_basic;
        public RepairingDockJson[] api_ndock;
        public ShipJson[] api_ship;
        public int api_combined_flag;
        public CombinedFleetType CombinedFleet => (CombinedFleetType)Math.Max(api_combined_flag, 0);
        internal class EventObject
        {
            public bool api_m_flag2;
        }
        public EventObject api_event_object;
    }
}
