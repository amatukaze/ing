namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle
{
    class FriendShip : ModelBase, IParticipant
    {
        public Ship Ship { get; }

        public ShipInfo Info => Ship.Info;

        public int Level => Ship.Level;

        bool r_IsMVP;
        public bool IsMVP
        {
            get { return r_IsMVP; }
            internal set
            {
                r_IsMVP = value;
                OnPropertyChanged(nameof(IsMVP));
            }
        }

        public FriendShip(Ship rpShip)
        {
            Ship = rpShip;
        }
    }
}
