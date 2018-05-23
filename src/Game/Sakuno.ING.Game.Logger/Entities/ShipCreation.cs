using System;
using System.ComponentModel.DataAnnotations;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger.Entities
{
    public class ShipCreation
    {
        [Key]
        public DateTimeOffset TimeStamp { get; set; }
        public MaterialsEntity Consumption { get; set; }
        public bool IsLSC { get; set; }
        public ShipInfoId ShipBuilt { get; set; }
        public int EmptyDockCount { get; set; }
        public ShipInfoId Secretary { get; set; }
        public int SecretaryLevel { get; set; }
        public int AdmiralLevel { get; set; }
    }
}
