using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class ItemInfo : RawDataWrapper<RawItemInfo>, IID
    {
        public int ID => RawData.ID;
        public string Name => RawData.Name;

        public ItemInfo(RawItemInfo rpRawData) : base(rpRawData)
        {
            OnRawDataUpdated();
        }

        protected override void OnRawDataUpdated()
        {
            var rTranslatedName = StringResources.Instance.Extra?.GetItemName(ID);
            if (rTranslatedName != null)
                RawData.Name = rTranslatedName;
        }

        public override string ToString() => $"ID = {ID}, Name = \"{Name}\"";
    }
}
