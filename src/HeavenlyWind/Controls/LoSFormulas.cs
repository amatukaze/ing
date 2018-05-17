using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Controls
{
    class LoSFormulas : ListBox
    {
        static LoSFormulas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoSFormulas), new FrameworkPropertyMetadata(typeof(LoSFormulas)));
        }

        protected override void OnItemsSourceChanged(IEnumerable rpOldValue, IEnumerable rpNewValue)
        {
            base.OnItemsSourceChanged(rpOldValue, rpNewValue);

            if (rpNewValue == null || Items.Count == 0)
                return;

            SelectedItem = Items[(int)Preference.Instance.Game.MainFleetLoSFormula.Value];
        }
    }
}
