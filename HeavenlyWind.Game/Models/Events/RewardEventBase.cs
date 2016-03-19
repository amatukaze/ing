using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public abstract class RewardEventBase : SortieEvent
    {
        public abstract int TypeID { get; }
        public abstract MaterialType ID { get; }
        public abstract int Quantity { get; }

        public string Name => TypeID == 4 ? GetMaterialName() : GetItemName();

        internal protected RewardEventBase(RawMapExploration rpData) : base(rpData) { }

        string GetMaterialName()
        {
            switch (ID)
            {
                case MaterialType.Fuel: return StringResources.Instance.Main.Material_Fuel;
                case MaterialType.Bullet: return StringResources.Instance.Main.Material_Bullet;
                case MaterialType.Steel: return StringResources.Instance.Main.Material_Steel;
                case MaterialType.Bauxite: return StringResources.Instance.Main.Material_Bauxite;
                case MaterialType.InstantConstruction: return StringResources.Instance.Main.Material_InstantConstruction;
                case MaterialType.Bucket: return StringResources.Instance.Main.Material_Bucket;
                case MaterialType.DevelopmentMaterial: return StringResources.Instance.Main.Material_DevelopmentMaterial;
                case MaterialType.ImprovementMaterial: return StringResources.Instance.Main.Material_ImprovementMaterial;

                default: return StringResources.Instance.Main.Sortie_Event_Unknown;
            }
        }
        string GetItemName()
        {
            ItemInfo rItem;
            if (KanColleGame.Current.MasterInfo.Items.TryGetValue((int)ID, out rItem))
                return rItem.TranslatedName;
            else
                return StringResources.Instance.Main.Sortie_Event_Unknown;
        }
    }
}
