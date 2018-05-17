using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Sakuno.KanColle.Amatsukaze.Behaviors
{
    class TextBoxLoseFocusAfterPressingEnterBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            if (AssociatedObject != null)
            {
                base.OnAttached();

                AssociatedObject.KeyUp += AssociatedObject_KeyUp;
            }
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.KeyUp -= AssociatedObject_KeyUp;

                base.OnDetaching();
            }
        }

        void AssociatedObject_KeyUp(object sender, KeyEventArgs e)
        {
            var rTextBox = sender as TextBox;
            if (rTextBox != null && e.Key == Key.Return)
                rTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }
}
