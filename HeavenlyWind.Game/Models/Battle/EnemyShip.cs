namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle
{
    class EnemyShip : IParticipant
    {
        public ShipInfo Info { get; }

        public int Level { get; }

        public bool IsMVP => false;
        public Ship Ship => null;

        public EnemyShip(int rpID, int rpLevel)
        {
            Info = KanColleGame.Current.MasterInfo.Ships[rpID];
            Level = rpLevel;
        }

        public override string ToString() => $"{Info} Lv. {Level}";
    }
}
