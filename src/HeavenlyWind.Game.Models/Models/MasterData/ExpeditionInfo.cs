using System;
using System.Collections.Generic;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public class ExpeditionInfo : Calculated<IRawExpeditionInfo>
    {
        internal ExpeditionInfo(IRawExpeditionInfo raw, ITableProvider owner) : base(raw, owner) { }

        public override void Update(IRawExpeditionInfo raw) => throw new NotImplementedException();
    }
}
