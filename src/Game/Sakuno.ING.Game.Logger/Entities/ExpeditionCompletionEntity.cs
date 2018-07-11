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

        private int MaterialsAcquired_Fuel { get; set; }
        private int MaterialsAcquired_Bullet { get; set; }
        private int MaterialsAcquired_Steel { get; set; }
        private int MaterialsAcquired_Bauxite { get; set; }

        [NotMapped]
        public ItemRecord RewardItem1
        {
            get => new ItemRecord
            {
                ItemId = (UseItemId)RewardItem1_ItemId,
                Count = RewardItem1_Count
            };
            set
            {
                RewardItem1_ItemId = value.ItemId;
                RewardItem1_Count = value.Count;
            }
        }
        private int RewardItem1_ItemId { get; set; }
        private int RewardItem1_Count { get; set; }

        [NotMapped]
        public ItemRecord RewardItem2
        {
            get => new ItemRecord
            {
                ItemId = (UseItemId)RewardItem2_ItemId,
                Count = RewardItem2_Count
            };
            set
            {
                RewardItem2_ItemId = value.ItemId;
                RewardItem2_Count = value.Count;
            }
        }
        private int RewardItem2_ItemId { get; set; }
        private int RewardItem2_Count { get; set; }
    }
}
