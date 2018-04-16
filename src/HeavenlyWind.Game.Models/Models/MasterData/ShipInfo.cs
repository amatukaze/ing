using System;
using System.Collections.Generic;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public class ShipInfo : Calculated<IRawShipInfo>
    {
        internal ShipInfo(int id, ITableProvider owner) : base(id, owner) { }

        public override void Update(IRawShipInfo raw) => throw new NotImplementedException();
    }
}
