namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Parsers
{
    class Result<T>
    {
        public T Value { get; }
        public string Rest { get; }

        public Result(T rpValue, string rpRest)
        {
            Value = rpValue;
            Rest = rpRest;
        }

        public override string ToString() => Rest;
    }
}
