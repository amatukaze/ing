using System.Linq;

namespace Sakuno.ING.Game.Models
{
    partial class Fleet
    {
        partial void UpdateCore(IRawFleet raw)
        {
            ships.Query = raw.ShipIds.Select(x => owner.AllShips[x]);
            Expedition = owner.MasterData.Expeditions[raw.ExpeditionId];
        }

        internal void ChangeComposition(int? index, Ship ship, Fleet fromFleet)
        {
            var oldShips = ships.ToList();
            if (index is int i)
            {
                if (ship != null)
                {
                    if (fromFleet != null)
                        if (i >= ships.Count)
                            fromFleet.ships.Remove(ship);
                        else
                            fromFleet.ships.Replace(ship, ships[i]);
                    ships[i] = ship;
                }
                else
                    ships.RemoveAt(i);
            }
            else
                while (ships.Count > 1)
                    ships.RemoveAt(1);
        }
    }
}
