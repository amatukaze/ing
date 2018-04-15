namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class NavalBase
    {
        public NavalBase(GameListener listener)
        {
            MasterData = new MasterDataRoot(listener);
        }

        public MasterDataRoot MasterData { get; }
    }
}
