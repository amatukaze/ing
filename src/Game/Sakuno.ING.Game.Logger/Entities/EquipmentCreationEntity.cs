using System.ComponentModel.DataAnnotations.Schema;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger.Entities
{
    public class EquipmentCreationEntity : EntityBase
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
            };
            set
            {
                Consumption_Fuel = value.Fuel;
                Consumption_Bullet = value.Bullet;
                Consumption_Steel = value.Steel;
                Consumption_Bauxite = value.Bauxite;
            }
        }

        private int Consumption_Fuel { get; set; }
        private int Consumption_Bullet { get; set; }
        private int Consumption_Steel { get; set; }
        private int Consumption_Bauxite { get; set; }

        public bool IsSuccess { get; set; }
        public EquipmentInfoId EquipmentCreated { get; set; }
        public ShipInfoId Secretary { get; set; }
        public int SecretaryLevel { get; set; }
        public int AdmiralLevel { get; set; }
    }
}
