using System.ComponentModel.DataAnnotations.Schema;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger.Entities
{
    public class ShipCreationEntity : EntityBase
    {
        [NotMapped]
        public Materials Consumption
        {
            get => new Materials
            {
                Fuel = Consumption_Fuel,
                Bullet = Consumption_Bullet,
                Steel = Consumption_Steel,
                Bauxite = Consumption_Bauxite,
                Development = Consumption_Development
            };
            set
            {
                Consumption_Fuel = value.Fuel;
                Consumption_Bullet = value.Bullet;
                Consumption_Steel = value.Steel;
                Consumption_Bauxite = value.Bauxite;
                Consumption_Development = value.Development;
            }
        }

        public int Consumption_Fuel { get; set; }
        public int Consumption_Bullet { get; set; }
        public int Consumption_Steel { get; set; }
        public int Consumption_Bauxite { get; set; }
        public int Consumption_Development { get; set; }

        [Column("ConstructionType")]
        public bool IsLSC { get; set; }
        public ShipInfoId ShipBuilt { get; set; }
        public int EmptyDockCount { get; set; }
        public ShipInfoId Secretary { get; set; }
        public int SecretaryLevel { get; set; }
        public int AdmiralLevel { get; set; }
    }
}
