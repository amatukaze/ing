using System;
using System.Collections.Generic;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public class EquipmentInfo : Calculated<IRawEquipmentInfo>
    {
        internal EquipmentInfo(int id, ITableProvider owner) : base(id, owner) { }

        public override void Update(IRawEquipmentInfo raw) => throw new NotImplementedException();
    }
}
