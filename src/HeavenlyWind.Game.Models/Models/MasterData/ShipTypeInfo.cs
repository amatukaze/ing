using System;
using System.Collections.Generic;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public class ShipTypeInfo : Calculated<IRawShipTypeInfo>
    {
        internal ShipTypeInfo(int id, ITableProvider owner) : base(id, owner) { }

        public override void Update(IRawShipTypeInfo raw) => throw new NotImplementedException();
    }
}
