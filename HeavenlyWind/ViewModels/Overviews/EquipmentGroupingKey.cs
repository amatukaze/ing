using System;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    public struct EquipmentGroupingKey : IEquatable<EquipmentGroupingKey>
    {
        public int Level { get; }
        public int Proficiency { get; }

        public EquipmentGroupingKey(int rpLevel, int rpProficiency)
        {
            Level = rpLevel;
            Proficiency = rpProficiency;
        }

        public bool Equals(EquipmentGroupingKey rpOther) => Level == rpOther.Level && Proficiency == rpOther.Proficiency;
        public override bool Equals(object rpObject) => rpObject != null ? Equals((EquipmentGroupingKey)rpObject) : false;
        public override int GetHashCode() => Level * 10 + Proficiency;

        public static bool operator ==(EquipmentGroupingKey x, EquipmentGroupingKey y) => x.Equals(y);
        public static bool operator !=(EquipmentGroupingKey x, EquipmentGroupingKey y) => !x.Equals(y);
    }
}
