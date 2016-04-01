using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class ExpeditionInfo : RawDataWrapper<RawExpeditionInfo>, IID, ITranslatedName
    {
        public static ExpeditionInfo Dummy { get; } = new ExpeditionInfo(new RawExpeditionInfo() { ID = -1, Name = "?????" });

        public int ID => RawData.ID;

        public MapAreaInfo MapArea => KanColleGame.Current.MasterInfo.MapAreas[RawData.MapAreaID];

        public string Name => RawData.Name;
        public string TranslatedName => StringResources.Instance.Extra?.GetExpeditionName(ID) ?? Name;
        public string Description => RawData.Description;

        public int Time => RawData.Time;

        public double FuelConsumption => RawData.FuelConsumption;
        public double BulletConsumption => RawData.BulletConsumption;

        public int RewardItem1ID => RawData.RewardItem1[0];
        public int RewardItem1Count => RawData.RewardItem1[1];
        public int RewardItem2ID => RawData.RewardItem2[0];
        public int RewardItem2Count => RawData.RewardItem2[1];

        public bool CanReturn => RawData.CanReturn;

        internal ExpeditionInfo(RawExpeditionInfo rpRawData) : base(rpRawData) { }

        public override string ToString() => $"ID = {ID}, Name = \"{Name}\"";
    }
}
