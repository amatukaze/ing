using Microsoft.EntityFrameworkCore;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Logger.Entities
{
    [Owned]
    public class MaterialsEntity
    {
        public int Fuel { get; set; }
        public int Bullet { get; set; }
        public int Steel { get; set; }
        public int Bauxite { get; set; }
        public int InstantBuild { get; set; }
        public int InstantRepair { get; set; }
        public int Development { get; set; }
        public int Improvement { get; set; }

        public static implicit operator MaterialsEntity(Materials materials)
            => new MaterialsEntity
            {
                Fuel = materials.Fuel,
                Bullet = materials.Bullet,
                Steel = materials.Steel,
                Bauxite = materials.Bauxite,
                Development = materials.Development,
                Improvement = materials.Improvement,
                InstantBuild = materials.InstantBuild,
                InstantRepair = materials.InstantRepair
            };

        public static implicit operator Materials(MaterialsEntity entity)
            => new Materials
            {
                Fuel = entity.Fuel,
                Bullet = entity.Bullet,
                Steel = entity.Steel,
                Bauxite = entity.Bauxite,
                Development = entity.Development,
                Improvement = entity.Improvement,
                InstantBuild = entity.InstantBuild,
                InstantRepair = entity.InstantRepair
            };
    }
}
