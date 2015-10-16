using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Functions;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Updaters
{

    class SetUpdater : Updater
    {
        public SetUpdater(Function rpFunction) : base(rpFunction) { }

        public override string ToString() => "Progress = " + Function.ToString();
    }
}
