using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class Equipment : RawDataWrapper<RawEquipment>, IID
    {
        public static Equipment Dummy { get; } = new Equipment(new RawEquipment() { EquipmentID = -1 });

        public int ID => RawData.ID;

        public EquipmentInfo Info { get; private set; }

        public int Level => RawData.Level;
        public string LevelText => Level == 0 ? string.Empty : Level == 10 ? "★max" : "★+" + Level;

        public bool IsLocked => RawData.IsLocked;

        public int? Proficiency => RawData.Proficiency;

        public string FullName => $"{Info.Name}{(Level == 0 ? string.Empty : $" {LevelText} ")}{(!Proficiency.HasValue ? string.Empty : $" (熟練度 {Proficiency.Value})")}";

        internal Equipment(RawEquipment rpRawData) : base(rpRawData)
        {
            EquipmentInfo rInfo;
            if (KanColleGame.Current.MasterInfo.Equipments.TryGetValue(rpRawData.EquipmentID, out rInfo))
                Info = rInfo;
            else
                Info = EquipmentInfo.Dummy;
        }

        public override string ToString() => $"ID = {ID}, Name = \"{Info.Name}\", Level = {Level}{(!Proficiency.HasValue ? string.Empty : $" Proficiency = {Proficiency}")}";
    }
}
