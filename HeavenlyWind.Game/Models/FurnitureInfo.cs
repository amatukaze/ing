using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class FurnitureInfo : RawDataWrapper<RawFurnitureInfo>, IID, ITranslatedName
    {
        public int ID => RawData.ID;

        public string Name => RawData.Name;
        public string TranslatedName => StringResources.Instance.Extra?.GetFurnitureName(ID) ?? Name;

        public FurnitureInfo(RawFurnitureInfo rpRawData) : base(rpRawData) { }

        public override string ToString() => $"ID = {ID}, Name = \"{Name}\"";
    }
}
