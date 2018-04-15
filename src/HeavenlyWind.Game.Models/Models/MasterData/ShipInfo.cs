using System;
using System.Collections.Generic;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public class ShipInfo : Calculated<IRawShipInfo>
    {
        internal ShipInfo(IRawShipInfo raw, ITableProvider owner) : base(raw, owner) { }

        public override void Update(IRawShipInfo raw) => throw new NotImplementedException();
    }
}
