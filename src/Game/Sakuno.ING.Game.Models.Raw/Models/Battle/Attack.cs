namespace Sakuno.ING.Game.Models.Battle
{
    public class Attack
    {
        public int? SourceIndex { get; }
        public int DestinationIndex { get; }
        public bool IsEnemy { get; }
        public int Type { get; }
    }
}
