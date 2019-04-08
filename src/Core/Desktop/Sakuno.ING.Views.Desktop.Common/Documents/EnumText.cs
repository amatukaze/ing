using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Localization;

namespace Sakuno.ING.Views.Desktop.Documents
{
    public class EnumText : Run
    {
        public static readonly DependencyProperty SourceProperty
            = DependencyProperty.Register(nameof(Source), typeof(Enum), typeof(EnumText),
                new PropertyMetadata(null, (d, e)
                    => ((EnumText)d).Update()));

        public Enum Source
        {
            get => (Enum)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private void Update() => Text = Source switch
        {
            AdmiralRank admiralRank => admiralRankText.Value.GetString(admiralRank),
            FireRange fireRange => fireRangeText.Value.GetString(fireRange),
            ShipSpeed shipSpeed => shipSpeedText.Value.GetString(shipSpeed),
            MapEventKind kind => mapEventText.Value.GetString(kind),
            BattleKind kind => battleKindText.Value.GetString(kind),
            BattleRank rank => Compositor.Static<ILocalizationService>().GetLocalized("Combat", "BattleRank_" + rank),
            Formation formation => Compositor.Static<ILocalizationService>().GetLocalized("Combat", "Formation_" + (int)formation),
            Engagement engagement => Compositor.Static<ILocalizationService>().GetLocalized("Combat", "Engagement_" + (int)engagement),
            AirFightingResult result => Compositor.Static<ILocalizationService>().GetLocalized("Combat", "AirFightingResult_" + (int)result),
            _ => null
        };

        private static readonly Lazy<ValueHolder<AdmiralRank>> admiralRankText = new Lazy<ValueHolder<AdmiralRank>>(() => new ValueHolder<AdmiralRank>("GameModel"));
        private static readonly Lazy<ValueHolder<FireRange>> fireRangeText = new Lazy<ValueHolder<FireRange>>(() => new ValueHolder<FireRange>("GameModel"));
        private static readonly Lazy<ValueHolder<ShipSpeed>> shipSpeedText = new Lazy<ValueHolder<ShipSpeed>>(() => new ValueHolder<ShipSpeed>("GameModel"));
        private static readonly Lazy<ValueHolder<MapEventKind>> mapEventText = new Lazy<ValueHolder<MapEventKind>>(() => new ValueHolder<MapEventKind>("Combat"));
        private static readonly Lazy<ValueHolder<BattleKind>> battleKindText = new Lazy<ValueHolder<BattleKind>>(() => new ValueHolder<BattleKind>("Combat"));

        private class ValueHolder<T>
            where T : Enum
        {
            public ValueHolder(string category)
            {
                var localization = Compositor.Static<ILocalizationService>();
                translated = Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(x => x, x => localization.GetLocalized(category, typeof(T).Name + "_" + Convert.ToInt32(x)));
            }

            private readonly Dictionary<T, string> translated;

            public string GetString(T value)
                => translated.TryGetValue(value, out string str) ? str : null;
        }
    }
}
