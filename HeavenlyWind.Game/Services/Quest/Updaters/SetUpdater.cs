using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Functions;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Updaters
{

    class SetUpdater : Updater
    {
        public SetUpdater(Function rpFunction) : base(rpFunction) { }

        public override void Invoke(ProgressInfo rpProgressInfo)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => "Progress = " + Function.ToString();
    }
}
