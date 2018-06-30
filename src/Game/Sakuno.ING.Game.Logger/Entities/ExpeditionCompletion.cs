using System;
using System.ComponentModel.DataAnnotations;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger.Entities
{
    public class ExpeditionCompletion : ITimedEntity
    {
        [Key]
        public DateTimeOffset TimeStamp { get; set; }
        public ExpeditionId ExpeditionId { get; set; }
        public string ExpeditionName { get; set; }
        public ExpeditionResult Result { get; set; }
        public MaterialsEntity MaterialsAcquired { get; set; }
        public ItemRecordEntity RewardItem1 { get; set; }
        public ItemRecordEntity RewardItem2 { get; set; }
    }
}
