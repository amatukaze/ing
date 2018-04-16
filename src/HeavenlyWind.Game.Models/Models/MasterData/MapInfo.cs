using System;
using System.Collections.Generic;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public class MapInfo : Calculated<IRawMapInfo>
    {
        internal MapInfo(int id, ITableProvider owner) : base(id, owner) { }

        public override void Update(IRawMapInfo raw) => throw new NotImplementedException();
    }
}
