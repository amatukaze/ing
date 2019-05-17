using System;

namespace Sakuno.ING.Game.Models
{
    public struct Materials : IEquatable<Materials>
    {
        /// <summary>
        /// 燃料
        /// </summary>
        public int Fuel { get; set; }
        /// <summary>
        /// 弾薬
        /// </summary>
        public int Bullet { get; set; }
        /// <summary>
        /// 鋼材
        /// </summary>
        public int Steel { get; set; }
        /// <summary>
        /// ボーキサイト
        /// </summary>
        public int Bauxite { get; set; }
        /// <summary>
        /// 高速建造材
        /// </summary>
        public int InstantBuild { get; set; }
        /// <summary>
        /// 高速修復材
        /// </summary>
        public int InstantRepair { get; set; }
        /// <summary>
        /// 開発資材
        /// </summary>
        public int Development { get; set; }
        /// <summary>
        /// 改修資材
        /// </summary>
        public int Improvement { get; set; }

        public static Materials operator +(in Materials left, in Materials right)
            => new Materials
            {
                Fuel = left.Fuel + right.Fuel,
                Bullet = left.Bullet + right.Bullet,
                Steel = left.Steel + right.Steel,
                Bauxite = left.Bauxite + right.Bauxite,
                InstantBuild = left.InstantBuild + right.InstantBuild,
                InstantRepair = left.InstantRepair + right.InstantRepair,
                Development = left.Development + right.Development,
                Improvement = left.Improvement + right.Improvement
            };

        public static Materials operator -(in Materials left, in Materials right)
            => new Materials
            {
                Fuel = left.Fuel - right.Fuel,
                Bullet = left.Bullet - right.Bullet,
                Steel = left.Steel - right.Steel,
                Bauxite = left.Bauxite - right.Bauxite,
                InstantBuild = left.InstantBuild - right.InstantBuild,
                InstantRepair = left.InstantRepair - right.InstantRepair,
                Development = left.Development - right.Development,
                Improvement = left.Improvement - right.Improvement
            };

        public static Materials operator *(in Materials value, int multiplier)
            => new Materials
            {
                Fuel = value.Fuel * multiplier,
                Bullet = value.Bullet * multiplier,
                Steel = value.Steel * multiplier,
                Bauxite = value.Bauxite * multiplier,
                InstantBuild = value.InstantBuild * multiplier,
                InstantRepair = value.InstantRepair * multiplier,
                Development = value.Development * multiplier,
                Improvement = value.Improvement * multiplier
            };

        public static Materials operator *(in Materials value, double multiplier)
            => new Materials
            {
                Fuel = (int)(value.Fuel * multiplier),
                Bullet = (int)(value.Bullet * multiplier),
                Steel = (int)(value.Steel * multiplier),
                Bauxite = (int)(value.Bauxite * multiplier),
                InstantBuild = (int)(value.InstantBuild * multiplier),
                InstantRepair = (int)(value.InstantRepair * multiplier),
                Development = (int)(value.Development * multiplier),
                Improvement = (int)(value.Improvement * multiplier)
            };

        public static bool operator ==(in Materials left, in Materials right)
            => left.Fuel == right.Fuel
            && left.Bullet == right.Bullet
            && left.Steel == right.Steel
            && left.Bauxite == right.Bauxite
            && left.InstantBuild == right.InstantBuild
            && left.InstantRepair == right.InstantRepair
            && left.Development == right.Development
            && left.Improvement == right.Improvement;

        public static bool operator !=(in Materials left, in Materials right)
            => !(left == right);

        public bool Equals(Materials other) => this == other;

        public override bool Equals(object other)
            => other is Materials m && this == m;

        public override int GetHashCode()
            => Fuel ^ Bullet ^ Steel ^ Bauxite
            ^ InstantBuild ^ InstantRepair ^ Development ^ Improvement;
    }
}
