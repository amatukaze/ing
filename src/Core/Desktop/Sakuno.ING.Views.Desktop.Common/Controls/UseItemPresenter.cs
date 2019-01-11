using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Views.Desktop.Controls
{
    [TemplatePart(Name = Name_PART_Difference, Type = typeof(TextBlock))]
    public class UseItemPresenter : Control
    {
        private const string Name_PART_Difference = "PART_Difference";

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

        static UseItemPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UseItemPresenter), new FrameworkPropertyMetadata(typeof(UseItemPresenter)));
        }

        private TextBlock _differenceText;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _differenceText = GetTemplateChild(Name_PART_Difference) as TextBlock;
        }

        private int _difference;
        private void Update(int oldValue, int newValue)
        {
            _difference += newValue - oldValue;
            _differenceText.Text = _difference.ToString("+0;-0;0");

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
            _differenceText.Text = string.Empty;
        }
    }
}
