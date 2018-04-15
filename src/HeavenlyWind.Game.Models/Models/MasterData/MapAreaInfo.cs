using System;
using System.Collections.Generic;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public class MapAreaInfo : Calculated<IRawMapArea>
    {
        internal MapAreaInfo(IRawMapArea raw, ITableProvider owner) : base(raw, owner) { }

        public override void Update(IRawMapArea raw) => throw new NotImplementedException();
    }
}
