using System;
using System.Collections.Generic;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public class MapAreaInfo : Calculated<IRawMapArea>
    {
        internal MapAreaInfo(int id, ITableProvider owner) : base(id, owner) { }

        public override void Update(IRawMapArea raw) => throw new NotImplementedException();
    }
}
