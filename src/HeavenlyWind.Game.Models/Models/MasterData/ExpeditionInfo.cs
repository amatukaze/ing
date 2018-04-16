using System;
using System.Collections.Generic;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public class ExpeditionInfo : Calculated<IRawExpeditionInfo>
    {
        internal ExpeditionInfo(int id, ITableProvider owner) : base(id, owner) { }

        public override void Update(IRawExpeditionInfo raw) => throw new NotImplementedException();
    }
}
