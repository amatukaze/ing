using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Controls
{
    class FighterPower : ListBox
    {
        static FighterPower()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FighterPower), new FrameworkPropertyMetadata(typeof(FighterPower)));
        }

        protected override void OnItemsSourceChanged(IEnumerable rpOldValue, IEnumerable rpNewValue)
        {
            base.OnItemsSourceChanged(rpOldValue, rpNewValue);

            if (rpNewValue == null || Items.Count == 0)
                return;

            SelectedItem = Items[(int)Preference.Instance.Game.MainFighterPowerFormula.Value];
        }
    }
}
