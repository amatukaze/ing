using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Functions;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Updaters
{
    class IncrementUpdater : Updater
    {
        public IncrementUpdater(Function rpFunction) : base(rpFunction) { }

        public override void Invoke(ProgressInfo rpProgressInfo)
        {
            rpProgressInfo.Progress++;
        }

        public override string ToString() => "Progress += " + Function.ToString();
    }
}
