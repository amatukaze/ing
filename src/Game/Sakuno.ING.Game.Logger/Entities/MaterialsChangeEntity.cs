using System.ComponentModel.DataAnnotations.Schema;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Logger.Entities
{
    public class MaterialsChangeEntity : EntityBase
    {
        public int Materials_Fuel { get; set; }
        public int Materials_Bullet { get; set; }
        public int Materials_Steel { get; set; }
        public int Materials_Bauxite { get; set; }
        public int Materials_InstantBuild { get; set; }
        public int Materials_InstantRepair { get; set; }
        public int Materials_Development { get; set; }
        public int Materials_Improvement { get; set; }

        [NotMapped]
        public Materials Materials
        {
            get => new Materials
            {
                Fuel = Materials_Fuel,
                Bullet = Materials_Bullet,
                Steel = Materials_Steel,
                Bauxite = Materials_Bauxite,
                InstantBuild = Materials_InstantBuild,
                InstantRepair = Materials_InstantRepair,
                Development = Materials_Development,
                Improvement = Materials_Improvement
            };
            set
            {
                Materials_Fuel = value.Fuel;
                Materials_Bullet = value.Bullet;
                Materials_Steel = value.Steel;
                Materials_Bauxite = value.Bauxite;
                Materials_InstantBuild = value.InstantBuild;
                Materials_InstantRepair = value.InstantRepair;
                Materials_Development = value.Development;
                Materials_Improvement = value.Improvement;
            }
        }

        public MaterialsChangeReason Reason { get; set; }
    }
}
