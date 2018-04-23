namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    partial class Equipment
    {
        private bool _isAvailable;
        public bool IsAvailable
        {
            get => _isAvailable;
            internal set => Set(ref _isAvailable, value);
        }

        partial void UpdateCore(IRawEquipment raw)
        {
            Info = equipmentInfos[raw.EquipmentInfoId];
        }
    }
}
