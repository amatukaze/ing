using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class Headquarter : RawDataWrapper<RawBasic>
    {
        public Admiral Admiral { get; }

        internal Headquarter(RawBasic rpRawData) : base(rpRawData)
        {
            Admiral = new Admiral(rpRawData);
        }

        protected override void OnRawDataUpdated()
        {
            Admiral.Update(RawData);
        }
    }
}
