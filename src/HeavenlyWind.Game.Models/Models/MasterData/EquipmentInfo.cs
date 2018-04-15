using System;
using System.Collections.Generic;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public class EquipmentInfo : Calculated<IRawEquipmentInfo>
    {
        internal EquipmentInfo(IRawEquipmentInfo raw) : base(raw) { }

        public override void Update(IRawEquipmentInfo raw) => throw new NotImplementedException();
    }
}
