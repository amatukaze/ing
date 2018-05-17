namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class Experience : IID
    {
        public int Level { get; }
        public int Total { get; }
        public int Next { get; }

        int IID.ID => Level;

        internal Experience(int rpLevel, int rpTotal, int rpNext)
        {
            Level = rpLevel;
            Total = rpTotal;
            Next = rpNext;
        }
    }
}
