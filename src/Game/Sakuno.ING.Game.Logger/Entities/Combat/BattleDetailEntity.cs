namespace Sakuno.ING.Game.Logger.Entities.Combat
{
    public class BattleDetailEntity : EntityBase
    {
        public byte[] SortieFleetState { get; set; }
        public byte[] SortieFleet2State { get; set; }
        public byte[] SupportFleetState { get; set; }
        public byte[] LbasState { get; set; }
        public byte[] LandBaseDefence { get; set; }
        public byte[] FirstBattleDetail { get; set; }
        public byte[] SecondBattleDetail { get; set; }
    }
}
