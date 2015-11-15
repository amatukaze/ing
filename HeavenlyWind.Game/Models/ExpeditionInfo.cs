using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class ExpeditionInfo : RawDataWrapper<RawExpeditionInfo>, IID
    {
        public int ID => RawData.ID;

        public MapAreaInfo MapArea => KanColleGame.Current.MasterInfo.MapAreas[RawData.MapAreaID];

        public string Name => RawData.Name;
        public string Description => RawData.Description;

        public TimeSpan Time => TimeSpan.FromMinutes(RawData.Time);

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
