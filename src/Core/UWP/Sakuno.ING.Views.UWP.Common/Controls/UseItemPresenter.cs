using System;
using System.Threading.Tasks;
using Sakuno.ING.Game.Models.Knowledge;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Controls
{
    [TemplatePart(Name = nameof(PART_Difference), Type = typeof(TextBlock))]
    public class UseItemPresenter : Control
    {
        public static readonly DependencyProperty IdProperty
            = DependencyProperty.Register(nameof(Id), typeof(KnownUseItem), typeof(UseItemPresenter), new PropertyMetadata(default(KnownUseItem)));

        public KnownUseItem Id
        {
            get => (KnownUseItem)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        public static readonly DependencyProperty AmountProperty =
            DependencyProperty.Register(nameof(Amount), typeof(int), typeof(UseItemPresenter),
                new PropertyMetadata(0, (d, e) => ((UseItemPresenter)d).Update((int)e.OldValue, (int)e.NewValue)));

        public int Amount
        {
            get => (int)GetValue(AmountProperty);
            set => SetValue(AmountProperty, value);
        }

        public UseItemPresenter()
        {
            DefaultStyleKey = typeof(UseItemPresenter);
        }

        private TextBlock PART_Difference;
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Difference = GetTemplateChild(nameof(PART_Difference)) as TextBlock;
        }

        private int _difference;
        private void Update(int oldValue, int newValue)
        {
            _difference += newValue - oldValue;
            PART_Difference.Text = _difference.ToString("+0;-0;0");

            UpdateVisualState();
        }

        private int _updateNestingCount;
        private async void UpdateVisualState()
        {
            const int ThrottleTime = 3000;

            VisualStateManager.GoToState(this, "ValueChanged", true);

            _updateNestingCount++;
            await Task.Delay(ThrottleTime);
            _updateNestingCount--;

            if (_updateNestingCount > 0)
                return;

            VisualStateManager.GoToState(this, "Normal", true);

            _difference = 0;
            PART_Difference.Text = string.Empty;
        }
    }
}
