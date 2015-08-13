using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class EquipmentTypeInfo : RawDataWrapper<RawEquipmentTypeInfo>, IID
    {
        public static EquipmentTypeInfo Dummy { get; } = new EquipmentTypeInfo(new RawEquipmentTypeInfo() { ID = -1, Name = "?" });

        public int ID => RawData.ID; 

        public string Name => RawData.Name;

        internal EquipmentTypeInfo(RawEquipmentTypeInfo rpRawData) : base(rpRawData) { }

        public override string ToString() => $"ID = {ID}, Name = \"{Name}\"";
    }
}
