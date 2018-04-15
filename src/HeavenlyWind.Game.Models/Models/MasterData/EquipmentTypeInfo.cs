using System;
using System.Collections.Generic;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public class EquipmentTypeInfo : Calculated<IRawEquipmentTypeInfo>
    {
        internal EquipmentTypeInfo(IRawEquipmentTypeInfo raw, ITableProvider owner) : base(raw, owner) { }

        public override void Update(IRawEquipmentTypeInfo raw) => throw new NotImplementedException();
    }
}
