using Avalonia.Automation;
using Avalonia.Data;
using static Sakuno.ING.Shell.UIComponentAutomation.Tests.SessionCoordinatorTests;

namespace Sakuno.ING.Shell.UIComponentAutomation.Tests;

public class TextInputScenarioTests
{
    [Fact]
    public async Task ReusableControlSupportsInspectInputSnapshotAndCapture()
    {
        var state = new SampleTextState();
        var definition = CreateDefinition(state);
        using var coordinator = new UiComponentSessionCoordinator(new UiComponentScenarioCatalog(definition));
        await coordinator.OpenAsync(definition.Id);

        var before = await coordinator.InspectAsync();
        var input = Assert.Single(before.Controls, control => control.AutomationId == "SampleInput");
        Assert.Equal(typeof(TextBox).FullName, input.Type);
        Assert.Equal(string.Empty, input.Text);
        Assert.True(input.IsVisible);
        Assert.True(input.IsEnabled);
        Assert.True(input.Bounds.Width > 0);

        await coordinator.InputTextAsync("SampleInput", "Bound text");

        Assert.Equal("Bound text", state.Text);
        var after = await coordinator.InspectAsync();
        Assert.Equal("Bound text", after.State["Text"]);
        Assert.Equal("Bound text", Assert.Single(after.Controls, control => control.AutomationId == "SampleInput").Text);
        var capture = await coordinator.CaptureScreenshotAsync();
        Assert.Equal(definition.Id, capture.ScenarioId);
        Assert.Equal(800, capture.Width);
        Assert.Equal(450, capture.Height);
        Assert.NotEmpty(capture.Png);
    }

    [Fact]
    public async Task InvalidTextTargetsReturnDistinctDiagnosticsWithoutChangingBinding()
    {
        var state = new SampleTextState();
        var definition = CreateDefinition(state);
        using var coordinator = new UiComponentSessionCoordinator(new UiComponentScenarioCatalog(definition));
        await coordinator.OpenAsync(definition.Id);
        await coordinator.InputTextAsync("SampleInput", "Before");

        var disabled = await AssertError(
            UiComponentAutomationErrorCodes.ControlDisabled,
            () => coordinator.InputTextAsync("DisabledInput", "Changed"));
        var invisible = await AssertError(
            UiComponentAutomationErrorCodes.ControlInvisible,
            () => coordinator.InputTextAsync("InvisibleInput", "Changed"));
        var ambiguous = await AssertError(
            UiComponentAutomationErrorCodes.AmbiguousControl,
            () => coordinator.InputTextAsync("DuplicateInput", "Changed"));
        var missing = await AssertError(
            UiComponentAutomationErrorCodes.ControlNotFound,
            () => coordinator.InputTextAsync("MissingInput", "Changed"));
        var notText = await AssertError(
            UiComponentAutomationErrorCodes.InoperableControl,
            () => coordinator.InputTextAsync("NotText", "Changed"));

        Assert.Equal("control_disabled", disabled.Error.Details["reason"]);
        Assert.Equal(false, disabled.Error.Details["isEnabled"]);
        Assert.Equal("control_invisible", invisible.Error.Details["reason"]);
        Assert.Equal(false, invisible.Error.Details["isVisible"]);
        Assert.Equal("ambiguous_control", ambiguous.Error.Details["reason"]);
        Assert.Equal(2, ambiguous.Error.Details["matchCount"]);
        Assert.Equal("control_not_found", missing.Error.Details["reason"]);
        Assert.Equal("not_text_input", notText.Error.Details["reason"]);
        Assert.Equal(typeof(Button).FullName, notText.Error.Details["controlType"]);

        await AssertError(UiComponentAutomationErrorCodes.ControlDisabled, () => coordinator.ClickAsync("DisabledInput"));
        await AssertError(UiComponentAutomationErrorCodes.ControlInvisible, () => coordinator.ClickAsync("InvisibleInput"));

        Assert.Equal("Before", state.Text);
        Assert.Equal("Before", (await coordinator.InspectAsync()).State["Text"]);
        Assert.NotEmpty((await coordinator.CaptureScreenshotAsync()).Png);
    }

    private static ScenarioDefinition CreateDefinition(SampleTextState state) =>
        new(
            "Tests/Controls/TextInput",
            "StackPanel",
            createContent: _ =>
            {
                var root = new StackPanel { DataContext = state, Width = 300 };
                root.Children.Add(CreateTextBox("SampleInput"));
                root.Children.Add(CreateTextBox("DisabledInput", isEnabled: false));
                root.Children.Add(CreateTextBox("InvisibleInput", isVisible: false));
                root.Children.Add(CreateTextBox("DuplicateInput"));
                root.Children.Add(CreateTextBox("DuplicateInput"));
                var button = new Button { Content = "Not text" };
                AutomationProperties.SetAutomationId(button, "NotText");
                root.Children.Add(button);
                return new(root, () => new Dictionary<string, object?> { ["Text"] = state.Text });
            });

    private static TextBox CreateTextBox(string automationId, bool isEnabled = true, bool isVisible = true)
    {
        var textBox = new TextBox
        {
            IsEnabled = isEnabled,
            IsVisible = isVisible,
            MinWidth = 160,
        };
        AutomationProperties.SetAutomationId(textBox, automationId);
        textBox.Bind(TextBox.TextProperty, new Binding(nameof(SampleTextState.Text))
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
        });
        return textBox;
    }

    private sealed class SampleTextState : System.ComponentModel.INotifyPropertyChanged
    {
        private string _text = string.Empty;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                PropertyChanged?.Invoke(this, new(nameof(Text)));
            }
        }
    }
}
