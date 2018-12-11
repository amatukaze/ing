using System.ComponentModel.DataAnnotations.Schema;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger.Entities
{
    public class ExpeditionCompletionEntity : EntityBase
    {
        public ExpeditionId ExpeditionId { get; set; }
        public string ExpeditionName { get; set; }
        public ExpeditionResult Result { get; set; }

        [NotMapped]
        public Materials MaterialsAcquired
        {
            get => new Materials
            {
                Fuel = MaterialsAcquired_Fuel,
                Bullet = MaterialsAcquired_Bullet,
                Steel = MaterialsAcquired_Steel,
                Bauxite = MaterialsAcquired_Bauxite,
            };
            set
            {
                MaterialsAcquired_Fuel = value.Fuel;
                MaterialsAcquired_Bullet = value.Bullet;
                MaterialsAcquired_Steel = value.Steel;
                MaterialsAcquired_Bauxite = value.Bauxite;
            }
        }

        public int MaterialsAcquired_Fuel { get; set; }
        public int MaterialsAcquired_Bullet { get; set; }
        public int MaterialsAcquired_Steel { get; set; }
        public int MaterialsAcquired_Bauxite { get; set; }

        [NotMapped]
        public UseItemRecord? RewardItem1
        {
            get => (RewardItem1_ItemId is int id
                && RewardItem1_Count is int count) ?
                new UseItemRecord
                {
                    ItemId = (UseItemId)id,
                    Count = count
                } : (UseItemRecord?)null;
            set
            {
                RewardItem1_ItemId = value?.ItemId;
                RewardItem1_Count = value?.Count;
            }
        }
        public int? RewardItem1_ItemId { get; set; }
        public int? RewardItem1_Count { get; set; }

        [NotMapped]
        public UseItemRecord? RewardItem2
        {
            get => (RewardItem2_ItemId is int id
                && RewardItem2_Count is int count) ?
                new UseItemRecord
                {
                    ItemId = (UseItemId)id,
                    Count = count
                } : (UseItemRecord?)null;
            set
            {
                RewardItem2_ItemId = value?.ItemId;
                RewardItem2_Count = value?.Count;
            }
        }
        public int? RewardItem2_ItemId { get; set; }
        public int? RewardItem2_Count { get; set; }
    }
}
