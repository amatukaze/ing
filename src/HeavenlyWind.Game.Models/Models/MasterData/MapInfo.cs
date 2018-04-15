using System;
using System.Collections.Generic;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    public class MapInfo : Calculated<IRawMapInfo>
    {
        internal MapInfo(IRawMapInfo raw, ITableProvider owner) : base(raw, owner) { }

        public override void Update(IRawMapInfo raw) => throw new NotImplementedException();
    }
}
