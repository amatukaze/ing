using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class Equipment : RawDataWrapper<RawEquipment>, IID
    {
        public static Equipment Dummy { get; } = new Equipment(new RawEquipment() { ID = -1, EquipmentID = -1 });
        static SortedList<int, Equipment> r_Dummies = new SortedList<int, Equipment>();

        public int ID => RawData.ID;

        public EquipmentInfo Info { get; private set; }

        public int Level => RawData.Level;
        public string LevelText => Level == 0 ? string.Empty : Level == 10 ? "★max" : "★+" + Level;

        bool r_IsLocked;
        public bool IsLocked
        {
            get { return r_IsLocked; }
            internal set
            {
                if (r_IsLocked != value)
                {
                    r_IsLocked = value;
                    OnPropertyChanged(nameof(IsLocked));
                }
            }
        }

        public int Proficiency => RawData.Proficiency;
        public string ProficiencyText => Proficiency == 0 ? string.Empty : "+" + Proficiency;

        public string FullName
        {
            get
            {
                var rBuilder = StringBuilderCache.Acquire();

                rBuilder.Append(Info.TranslatedName);

                if (Proficiency > 0)
                    rBuilder.Append(' ').Append(ProficiencyText);

                if (Level > 0)
                    rBuilder.Append(' ').Append(LevelText);

                return rBuilder.GetStringAndRelease();
            }
        }

        internal Equipment(RawEquipment rpRawData) : base(rpRawData)
        {
            Info = KanColleGame.Current.MasterInfo.Equipment.GetValueOrDefault(rpRawData.EquipmentID) ?? EquipmentInfo.Dummy;

            IsLocked = RawData.IsLocked;
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

            IsLocked = RawData.IsLocked;

            OnPropertyChanged(nameof(Info));
            OnPropertyChanged(nameof(Level));
            OnPropertyChanged(nameof(LevelText));
            OnPropertyChanged(nameof(Proficiency));
            OnPropertyChanged(nameof(ProficiencyText));
        }

        public override string ToString() => $"ID = {ID}, Name = \"{Info.Name}\", Level = {Level}{(Proficiency == 0 ? string.Empty : $" Proficiency = {Proficiency}")}";
    }
}
