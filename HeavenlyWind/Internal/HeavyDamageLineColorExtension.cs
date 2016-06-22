using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Sakuno.KanColle.Amatsukaze.Internal
{
    class HeavyDamageLineColorExtension : MarkupExtension
    {
        BattleParticipantState? r_State;

        public HeavyDamageLineColorExtension() { }
        public HeavyDamageLineColorExtension(BattleParticipantState rpState)
        {
            r_State = rpState;
        }

        public override object ProvideValue(IServiceProvider rpServiceProvider)
        {
            var rSource = Preference.Current.UI.HeavyDamageLine;
            var rResult = new MultiBinding()
            {
                Mode = BindingMode.OneWay,
                Converter = CoreConverter.Instance,
                Bindings = { new Binding(nameof(rSource.Type)) { Source = rSource } },
            };

            if (!r_State.HasValue)
                rResult.Bindings.Add(new Binding("State"));
            else
                rResult.ConverterParameter = r_State.Value;

            return rResult.ProvideValue(rpServiceProvider);
        }

        class CoreConverter : IMultiValueConverter
        {
            public static CoreConverter Instance { get; } = new CoreConverter();

            public object Convert(object[] rpValues, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
            {
                if (rpValues.Length > 1)
                    rpParameter = rpValues[1];

                if (rpValues[0] == DependencyProperty.UnsetValue)
                    return Binding.DoNothing;

                var rType = (HeavyDamageLineType)rpValues[0];
                switch (rType)
                {
                    case HeavyDamageLineType.Default:
                        switch ((BattleParticipantState)rpParameter)
                        {
                            case BattleParticipantState.Healthy:
                                return App.Current.TryFindResource("ActiveForegroundBrushKey") ?? Brushes.Transparent;

                            case BattleParticipantState.LightlyDamaged:
                            case BattleParticipantState.ModeratelyDamaged:
                                return App.Current.TryFindResource("ThemeBrushKey") ?? Brushes.Transparent;

                            default:
                                return Brushes.Transparent;
                        }

                    case HeavyDamageLineType.AllRed:
                        switch ((BattleParticipantState)rpParameter)
                        {
                            case BattleParticipantState.Healthy:
                            case BattleParticipantState.LightlyDamaged:
                            case BattleParticipantState.ModeratelyDamaged:
                                return Brushes.Red;

                            default:
                                return Brushes.Transparent;
                        }

                    case HeavyDamageLineType.None:
                    case HeavyDamageLineType.Custom:
                    default:
                        return Brushes.Transparent;
                }
            }

            public object[] ConvertBack(object rpValue, Type[] rpTargetTypes, object rpParameter, CultureInfo rpCulture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
