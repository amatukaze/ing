using System;
using System.Collections.Generic;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public class EquipmentTypeInfo : Calculated<IRawEquipmentTypeInfo>
    {
        internal EquipmentTypeInfo(IRawEquipmentTypeInfo raw) : base(raw) { }

        public override void Update(IRawEquipmentTypeInfo raw) => throw new NotImplementedException();
    }
}
