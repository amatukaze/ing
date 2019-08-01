using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger.Entities.Homeport
{
    public class BattleConsumptionEntity
    {
        [Key]
        public DateTimeOffset TimeStamp { get; set; }
        public MapId MapId { get; set; }

        #region Consumption
        public int Consumption_Fuel { get; set; }
        public int Consumption_Bullet { get; set; }
        public int Consumption_Steel { get; set; }
        public int Consumption_Bauxite { get; set; }
        public int Consumption_InstantBuild { get; set; }
        public int Consumption_InstantRepair { get; set; }
        public int Consumption_Development { get; set; }
        public int Consumption_Improvement { get; set; }

        [NotMapped]
        public Materials Consumption
        {
            get => new Materials
            {
                Fuel = Consumption_Fuel,
                Bullet = Consumption_Bullet,
                Steel = Consumption_Steel,
                Bauxite = Consumption_Bauxite,
                InstantBuild = Consumption_InstantBuild,
                InstantRepair = Consumption_InstantRepair,
                Development = Consumption_Development,
                Improvement = Consumption_Improvement
            };
            set
            {
                Consumption_Fuel = value.Fuel;
                Consumption_Bullet = value.Bullet;
                Consumption_Steel = value.Steel;
                Consumption_Bauxite = value.Bauxite;
                Consumption_InstantBuild = value.InstantBuild;
                Consumption_InstantRepair = value.InstantRepair;
                Consumption_Development = value.Development;
                Consumption_Improvement = value.Improvement;
            }
        }
        #endregion

        #region ActualConsumption
        public int ActualConsumption_Fuel { get; set; }
        public int ActualConsumption_Bullet { get; set; }
        public int ActualConsumption_Steel { get; set; }
        public int ActualConsumption_Bauxite { get; set; }
        public int ActualConsumption_InstantBuild { get; set; }
        public int ActualConsumption_InstantRepair { get; set; }
        public int ActualConsumption_Development { get; set; }
        public int ActualConsumption_Improvement { get; set; }

        [NotMapped]
        public Materials ActualConsumption
        {
            get => new Materials
            {
                Fuel = ActualConsumption_Fuel,
                Bullet = ActualConsumption_Bullet,
                Steel = ActualConsumption_Steel,
                Bauxite = ActualConsumption_Bauxite,
                InstantBuild = ActualConsumption_InstantBuild,
                InstantRepair = ActualConsumption_InstantRepair,
                Development = ActualConsumption_Development,
                Improvement = ActualConsumption_Improvement
            };
            set
            {
                ActualConsumption_Fuel = value.Fuel;
                ActualConsumption_Bullet = value.Bullet;
                ActualConsumption_Steel = value.Steel;
                ActualConsumption_Bauxite = value.Bauxite;
                ActualConsumption_InstantBuild = value.InstantBuild;
                ActualConsumption_InstantRepair = value.InstantRepair;
                ActualConsumption_Development = value.Development;
                ActualConsumption_Improvement = value.Improvement;
            }
        }
        #endregion

        #region Acquired
        public int Acquired_Fuel { get; set; }
        public int Acquired_Bullet { get; set; }
        public int Acquired_Steel { get; set; }
        public int Acquired_Bauxite { get; set; }
        public int Acquired_InstantBuild { get; set; }
        public int Acquired_InstantRepair { get; set; }
        public int Acquired_Development { get; set; }
        public int Acquired_Improvement { get; set; }

        [NotMapped]
        public Materials Acquired
        {
            get => new Materials
            {
                Fuel = Acquired_Fuel,
                Bullet = Acquired_Bullet,
                Steel = Acquired_Steel,
                Bauxite = Acquired_Bauxite,
                InstantBuild = Acquired_InstantBuild,
                InstantRepair = Acquired_InstantRepair,
                Development = Acquired_Development,
                Improvement = Acquired_Improvement
            };
            set
            {
                Acquired_Fuel = value.Fuel;
                Acquired_Bullet = value.Bullet;
                Acquired_Steel = value.Steel;
                Acquired_Bauxite = value.Bauxite;
                Acquired_InstantBuild = value.InstantBuild;
                Acquired_InstantRepair = value.InstantRepair;
                Acquired_Development = value.Development;
                Acquired_Improvement = value.Improvement;
            }
        }
        #endregion
    }
}
