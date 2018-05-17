namespace Sakuno.KanColle.Amatsukaze.Game.Proxy
{
    public class SessionHeader : ModelBase
    {
        public string Name { get; }
        public string Value { get; }

        public SessionHeader(string rpName, string rpValue)
        {
            Name = rpName;
            Value = rpValue;
        }

        public override string ToString() => $"{Name}: {Value}";
    }
}
