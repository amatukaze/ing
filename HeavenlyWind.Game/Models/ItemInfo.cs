using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class ItemInfo : RawDataWrapper<RawItemInfo>, IID, ITranslatedName
    {
        public int ID => RawData.ID;
        public string Name => RawData.Name;
        public string TranslatedName => StringResources.Instance.Extra?.GetItemName(ID) ?? Name;

        public ItemInfo(RawItemInfo rpRawData) : base(rpRawData) { }

        public override string ToString() => $"ID = {ID}, Name = \"{Name}\"";
    }
}
