namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Triggers
{
    class UnknownTrigger : Trigger
    {
        public static UnknownTrigger Instance { get; } = new UnknownTrigger();

        UnknownTrigger() { }

        public override string ToString() => "Unknown";
    }
}
