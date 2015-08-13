namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class Experience : IID
    {
        public int Level { get; private set; }
        public int Total { get; private set; }
        public int Next { get; private set; }

        int IID.ID => Level;

        internal Experience(int rpLevel, int rpTotal, int rpNext)
        {
            Level = rpLevel;
            Total = rpTotal;
            Next = rpNext;
        }
    }
}
