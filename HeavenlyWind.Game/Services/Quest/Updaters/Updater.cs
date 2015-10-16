using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Functions;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Updaters
{
    abstract class Updater
    {
        public static Updater Default { get; } = new IncrementUpdater(ConstantFunction.One);

        public Function Function { get; protected set; }

        protected Updater(Function rpFunction)
        {
            Function = rpFunction;
        }
    }
}
