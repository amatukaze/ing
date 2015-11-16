using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class WhirlpoolEvent : SortieEvent
    {
        public SortieItem LostItem { get; }
        public int Amount { get; }

        public bool HasReduceLossesWithRadar { get; }

        public string Name { get; }

        internal WhirlpoolEvent(RawMapExploration rpData) : base(rpData)
        {
            LostItem = rpData.Whirlpool.MaterialType;
            Amount = rpData.Whirlpool.Amount;

            if (LostItem == SortieItem.Fuel)
                Name = StringResources.Instance.Main.Material_Fuel;
            else
                Name = StringResources.Instance.Main.Material_Bullet;
        }
    }
}
