using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;
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

        private void Update()
        {
            switch (Source)
            {
                case AdmiralRank admiralRank:
                    Text = admiralRankText.Value.GetString(admiralRank);
                    break;
                case FireRange fireRange:
                    Text = fireRangeText.Value.GetString(fireRange);
                    break;
                case ShipSpeed shipSpeed:
                    Text = shipSpeedText.Value.GetString(shipSpeed);
                    break;
                default:
                    Text = null;
                    break;
            }
        }

        private static readonly Lazy<ValueHolder<AdmiralRank>> admiralRankText = new Lazy<ValueHolder<AdmiralRank>>();
        private static readonly Lazy<ValueHolder<FireRange>> fireRangeText = new Lazy<ValueHolder<FireRange>>();
        private static readonly Lazy<ValueHolder<ShipSpeed>> shipSpeedText = new Lazy<ValueHolder<ShipSpeed>>();

        private class ValueHolder<T>
            where T : Enum
        {
            public ValueHolder()
            {
                var localization = Compositor.Static<ILocalizationService>();
                translated = Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(x => x, x => localization.GetLocalized("GameModel", typeof(T).Name + "_" + Convert.ToInt32(x)));
            }

            private readonly Dictionary<T, string> translated;

            public string GetString(T value)
                => translated.TryGetValue(value, out string str) ? str : null;
        }
    }
}
