using Avalonia.Automation;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;

namespace Sakuno.ING.Shell.UIComponentAutomation;

public sealed class UiComponentSession : IDisposable
{
    private const int MaxInspectionDepth = 8;
    private const int MaxInspectionNodes = 200;

    private readonly ScenarioDefinition _definition;
    private readonly Window _window;
    private readonly IContainer _container;
    private readonly IResolverContext _scope;
    private readonly ScenarioContent _content;
    private bool _disposed;

    public string ScenarioId => _definition.Id;

    internal IResolverContext Scope => _scope;
    internal Control Root => _content.Root;

    private UiComponentSession(
        ScenarioDefinition definition,
        Window window,
        IContainer container,
        IResolverContext scope,
        ScenarioContent content)
    {
        _definition = definition;
        _window = window;
        _container = container;
        _scope = scope;
        _content = content;
    }

    internal static UiComponentSession Open(ScenarioDefinition definition) =>
        UiComponentHost.Dispatch(() =>
        {
            IContainer? container = null;
            IResolverContext? scope = null;
            Window? window = null;
            ScenarioContent? content = null;

            try
            {
                container = definition.CreateContainer();
                scope = container.OpenScope();
                window = new Window
                {
                    Width = definition.Width,
                    Height = definition.Height,
                    SizeToContent = SizeToContent.Manual,
                    Background = Brushes.White,
                };
                DependencyInjection.SetContainer(window, scope);

                content = definition.CreateContent(scope);
                window.Content = content.Root;
                window.Show();
                UiComponentHost.Settle(window);

                return new UiComponentSession(definition, window, container, scope, content);
            }
            catch
            {
                if (window is not null)
                {
                    window.Content = null;
                    window.ClearValue(DependencyInjection.ContainerProperty);
                    window.Close();
                }
                content?.OwnedState?.Dispose();
                scope?.Dispose();
                container?.Dispose();
                throw;
            }
        });

    public InspectionResult Inspect(string? rootIdentifier = null, int maxDepth = 4, int maxNodes = 100)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxDepth);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(maxDepth, MaxInspectionDepth);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxNodes, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(maxNodes, MaxInspectionNodes);

        return UiComponentHost.Dispatch(() =>
        {
            ThrowIfDisposed();
            UiComponentHost.Settle(_window);

            var root = Root;
            if (rootIdentifier is not null)
            {
                var matches = Resolve(rootIdentifier);
                root = matches.Count switch
                {
                    0 => throw UiComponentAutomationErrors.ControlNotFound(ScenarioId, rootIdentifier),
                    1 => matches[0],
                    _ => throw UiComponentAutomationErrors.AmbiguousControl(ScenarioId, rootIdentifier),
                };
            }

            var controls = new List<ControlInspection>();
            var truncated = false;

            Visit(root, 0);

            return new InspectionResult(ScenarioId, controls, _content.CaptureState(), truncated);

            void Visit(Control control, int depth)
            {
                if (controls.Count == maxNodes)
                {
                    truncated = true;
                    return;
                }

                controls.Add(Describe(control, depth));
                if (depth == maxDepth)
                    return;

                foreach (var child in control.GetVisualChildren().OfType<Control>())
                    Visit(child, depth + 1);
            }
        });
    }

    public ControlInteractionResult Click(string identifier) =>
        UiComponentHost.Dispatch<ControlInteractionResult>(() =>
        {
            ThrowIfDisposed();
            UiComponentHost.Settle(_window);

            var matches = Resolve(identifier);
            if (matches.Count == 0)
                return new(ControlInteractionStatus.NotFound, new("control_not_found", MatchCount: 0));
            if (matches.Count > 1)
                return new(ControlInteractionStatus.Ambiguous, new("ambiguous_control", MatchCount: matches.Count));

            var control = matches[0];
            if (!IsVisibleInTree(control))
                return new(ControlInteractionStatus.Invisible, DescribeInteraction(control, "control_invisible"));
            if (!control.IsEffectivelyEnabled)
                return new(ControlInteractionStatus.Disabled, DescribeInteraction(control, "control_disabled"));

            var origin = control.TranslatePoint(new(), _window)
                ?? throw new InvalidOperationException("The control is not attached to the scenario window.");
            var point = origin + new Point(control.Bounds.Width / 2, control.Bounds.Height / 2);

            _window.MouseMove(point, RawInputModifiers.None);
            _window.MouseDown(point, MouseButton.Left, RawInputModifiers.None);
            _window.MouseUp(point, MouseButton.Left, RawInputModifiers.None);
            UiComponentHost.Settle(_window);

            return new(ControlInteractionStatus.Success);
        });

    public ControlInteractionResult InputText(string identifier, string text) =>
        UiComponentHost.Dispatch<ControlInteractionResult>(() =>
        {
            ThrowIfDisposed();
            UiComponentHost.Settle(_window);

            var matches = Resolve(identifier);
            if (matches.Count == 0)
                return new(ControlInteractionStatus.NotFound, new("control_not_found", MatchCount: 0));
            if (matches.Count > 1)
                return new(ControlInteractionStatus.Ambiguous, new("ambiguous_control", MatchCount: matches.Count));

            var control = matches[0];
            if (!IsVisibleInTree(control))
                return new(ControlInteractionStatus.Invisible, DescribeInteraction(control, "control_invisible"));
            if (!control.IsEffectivelyEnabled)
                return new(ControlInteractionStatus.Disabled, DescribeInteraction(control, "control_disabled"));
            if (control is not TextBox)
                return new(ControlInteractionStatus.Inoperable, DescribeInteraction(control, "not_text_input"));

            var origin = control.TranslatePoint(new(), _window)
                ?? throw new InvalidOperationException("The control is not attached to the scenario window.");
            var point = origin + new Point(control.Bounds.Width / 2, control.Bounds.Height / 2);

            _window.MouseMove(point, RawInputModifiers.None);
            _window.MouseDown(point, MouseButton.Left, RawInputModifiers.None);
            _window.MouseUp(point, MouseButton.Left, RawInputModifiers.None);
            _window.KeyTextInput(text);
            UiComponentHost.Settle(_window);

            return new(ControlInteractionStatus.Success);
        });

    public ScreenshotCapture CaptureScreenshot() =>
        UiComponentHost.Dispatch<ScreenshotCapture>(() =>
        {
            ThrowIfDisposed();
            UiComponentHost.Settle(_window);

            using var frame = _window.CaptureRenderedFrame()
                ?? throw new InvalidOperationException("The scenario window has not rendered a frame.");
            using var stream = new MemoryStream();
            frame.Save(stream, PngBitmapEncoderOptions.Default);

            return new(ScenarioId, frame.PixelSize.Width, frame.PixelSize.Height, stream.ToArray());
        });

    private IReadOnlyList<Control> Resolve(string identifier) =>
        new[] { Root }
            .Concat(Root.GetVisualDescendants().OfType<Control>())
            .Where(control => AutomationProperties.GetAutomationId(control) == identifier
                || AutomationProperties.GetName(control) == identifier
                || control.Name == identifier)
            .Distinct()
            .ToArray();

    private static OperationDiagnostics DescribeInteraction(Control control, string reason) =>
        new(
            reason,
            control.GetType().FullName,
            IsVisible: IsVisibleInTree(control),
            IsEnabled: control.IsEffectivelyEnabled);

    private ControlInspection Describe(Control control, int depth)
    {
        var origin = control.TranslatePoint(new(), _window) ?? new();
        return new(
            depth,
            control.GetType().FullName!,
            AutomationProperties.GetAutomationId(control),
            AutomationProperties.GetName(control) ?? control.Name,
            control switch
            {
                TextBlock textBlock => textBlock.Text,
                TextBox textBox => textBox.Text,
                ContentControl { Content: string text } => text,
                _ => null,
            },
            IsVisibleInTree(control),
            control.IsEffectivelyEnabled,
            new(origin.X, origin.Y, control.Bounds.Width, control.Bounds.Height));
    }

    private static bool IsVisibleInTree(Control control) =>
        control.IsVisible && control.GetVisualAncestors().OfType<Visual>().All(visual => visual.IsVisible);

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);

    public void Dispose()
    {
        if (_disposed)
            return;

        UiComponentHost.Dispatch(() =>
        {
            _window.Close();
            _window.Content = null;
            _window.ClearValue(DependencyInjection.ContainerProperty);
            _content.OwnedState?.Dispose();
            _scope.Dispose();
            _container.Dispose();
            _disposed = true;
            return true;
        });
    }
}
