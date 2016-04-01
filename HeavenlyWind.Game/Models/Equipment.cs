using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Collections.Generic;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class Equipment : RawDataWrapper<RawEquipment>, IID
    {
        public static Equipment Dummy { get; } = new Equipment(new RawEquipment() { ID = -1, EquipmentID = -1 });
        static Dictionary<int, Equipment> r_Dummies = new Dictionary<int, Equipment>();

        public int ID => RawData.ID;

        public EquipmentInfo Info { get; private set; }

        public int Level => RawData.Level;
        public string LevelText => Level == 0 ? string.Empty : Level == 10 ? "★max" : "★+" + Level;

        public bool IsLocked => RawData.IsLocked;

        public int Proficiency => RawData.Proficiency;
        public string ProficiencyText => Proficiency == 0 ? string.Empty : "+" + Proficiency;

        public string FullName
        {
            get
            {
                var rBuilder = new StringBuilder();

                rBuilder.Append(Info.TranslatedName);

                if (Proficiency > 0)
                    rBuilder.Append(' ').Append(ProficiencyText);

                if (Level > 0)
                    rBuilder.Append(' ').Append(LevelText);

                return rBuilder.ToString();
            }
        }

        internal Equipment(RawEquipment rpRawData) : base(rpRawData)
        {
            Info = KanColleGame.Current.MasterInfo.Equipment.GetValueOrDefault(rpRawData.EquipmentID) ?? EquipmentInfo.Dummy;
        }

        public static Equipment GetDummy(int rpID)
        {
            Equipment rResult;
            if (!r_Dummies.TryGetValue(rpID, out rResult))
            {
                EquipmentInfo rMasterInfo;
                if (!KanColleGame.Current.MasterInfo.Equipment.TryGetValue(rpID, out rMasterInfo))
                    return null;

                r_Dummies.Add(rpID, rResult = new Equipment(new RawEquipment() { ID = -1, EquipmentID = rpID }));
            }

            return rResult;
        }

        protected override void OnRawDataUpdated()
        {
            if (Info == null || Info.ID != RawData.EquipmentID)
                Info = KanColleGame.Current.MasterInfo.Equipment.GetValueOrDefault(RawData.EquipmentID) ?? EquipmentInfo.Dummy;

            OnPropertyChanged(nameof(Info));
            OnPropertyChanged(nameof(Level));
            OnPropertyChanged(nameof(LevelText));
            OnPropertyChanged(nameof(IsLocked));
            OnPropertyChanged(nameof(Proficiency));
            OnPropertyChanged(nameof(ProficiencyText));
        }

        public override string ToString() => $"ID = {ID}, Name = \"{Info.Name}\", Level = {Level}{(Proficiency == 0 ? string.Empty : $" Proficiency = {Proficiency}")}";
    }
}
