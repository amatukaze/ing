using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class RewardEvent : SortieEvent
    {
        public int TypeID { get; set; }
        public SortieItem Item { get; }
        public int Quantity { get; }

        public string Name { get; }

        internal RewardEvent(RawMapExploration rpData) : base(rpData)
        {
            TypeID = rpData.Reward.TypeID;
            Item = rpData.Reward.Item;
            Quantity = rpData.Reward.Quantity;

            if (TypeID == 4)
                Name = GetMaterialName();
            else
                Name = StringResources.Instance.Main.Sortie_Event_Unknown;
        }

        string GetMaterialName()
        {
            switch (Item)
            {
                case SortieItem.Fuel: return StringResources.Instance.Main.Material_Fuel;
                case SortieItem.Bullet: return StringResources.Instance.Main.Material_Bullet;
                case SortieItem.Steel: return StringResources.Instance.Main.Material_Steel;
                case SortieItem.Bauxite: return StringResources.Instance.Main.Material_Bauxite;
                case SortieItem.InstantConstruction: return StringResources.Instance.Main.Material_InstantConstruction;
                case SortieItem.Bucket: return StringResources.Instance.Main.Material_Bucket;
                case SortieItem.DevelopmentMaterial: return StringResources.Instance.Main.Material_DevelopmentMaterial;
                case SortieItem.ImprovementMaterial: return StringResources.Instance.Main.Material_ImprovementMaterial;

                default: return StringResources.Instance.Main.Sortie_Event_Unknown;
            }
        }
    }
}
