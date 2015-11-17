using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class RewardEvent : SortieEvent
    {
        public int TypeID { get; set; }
        public MaterialType ID { get; }
        public int Quantity { get; }

        public string Name { get; }

        internal RewardEvent(RawMapExploration rpData) : base(rpData)
        {
            TypeID = rpData.Reward.TypeID;
            ID = rpData.Reward.ID;
            Quantity = rpData.Reward.Quantity;

            if (TypeID == 4)
                Name = GetMaterialName();
            else
                Name = StringResources.Instance.Main.Sortie_Event_Unknown;
        }

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
    }
}
