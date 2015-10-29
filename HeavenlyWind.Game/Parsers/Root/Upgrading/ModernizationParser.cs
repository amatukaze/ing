using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Upgrading
{
    [Api("api_req_kaisou/powerup")]
    class ModernizationParser : ApiParser<RawModernization>
    {
        public override void Process(RawModernization rpData)
        {
            var rShipsTable = Game.Port.Ships;

            var rShipID = int.Parse(Requests["api_id"]);
            Ship rTargetShip;
            if (rShipsTable.TryGetValue(rShipID, out rTargetShip))
                rTargetShip.Update(rpData.Ship);

            var rShips = Requests["api_id_items"].Split(',').Select(r =>
            {
                Ship rShip = null;
                rShipsTable.TryGetValue(int.Parse(r), out rShip);

                return rShip;
            }).Where(r => r != null);

            foreach (var rShip in rShips)
            {
                foreach (var rEquipment in rShip.Equipments)
                    Game.Port.Equipments.Remove(rEquipment);
                
                rShipsTable.Remove(rShip);
            }

            Game.Port.UpdateShipsCore();
            Game.Port.Fleets.Update(rpData.Fleets);
        }
    }
}
