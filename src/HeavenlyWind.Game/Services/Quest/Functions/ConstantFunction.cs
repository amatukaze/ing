namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Functions
{
    class ConstantFunction : Function
    {
        public static ConstantFunction One { get; } = new ConstantFunction(1);

        public int Value { get; }

        public ConstantFunction(int rpValue)
        {
            Value = rpValue;
        }

        public override string ToString() => "Constant: " + Value;
    }
}
