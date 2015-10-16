using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Triggers;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Updaters;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest
{
    class ProgressRule
    {
        public Trigger Trigger { get; }
        public Updater Updater { get; }

        public ProgressRule(Trigger rpTrigger, Updater rpUpdater)
        {
            Trigger = rpTrigger;
            Updater = rpUpdater;
        }

        public override string ToString() => $@"Trigger -> {Trigger}{Environment.NewLine}Updater -> {Updater}";
    }
}
