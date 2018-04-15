using System;
using System.Collections.Generic;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public class ShipTypeInfo : Calculated<IRawShipTypeInfo>
    {
        internal ShipTypeInfo(IRawShipTypeInfo raw, ITableProvider owner) : base(raw, owner) { }

        public override void Update(IRawShipTypeInfo raw) => throw new NotImplementedException();
    }
}
