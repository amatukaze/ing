namespace Sakuno.ING.Shell.UIComponentAutomation;

public sealed class ScenarioDefinition
{
    private readonly Action<IContainer> _registerServices;
    private readonly Func<IResolverContext, ScenarioContent> _createContent;

    public string Id { get; }
    public string ComponentName { get; }
    public int Width { get; }
    public int Height { get; }

    internal ScenarioDefinition(
        string id,
        string componentName,
        int width = 800,
        int height = 450,
        Action<IContainer>? registerServices = null,
        Func<IResolverContext, ScenarioContent>? createContent = null)
    {
        Id = id;
        ComponentName = componentName;
        Width = width;
        Height = height;
        _registerServices = registerServices ?? (_ => { });
        _createContent = createContent ?? throw new ArgumentNullException(nameof(createContent));
    }

    internal IContainer CreateContainer()
    {
        var container = new Container();
        _registerServices(container);
        return container;
    }

    internal ScenarioContent CreateContent(IResolverContext scope) => _createContent(scope);
}

internal sealed record ScenarioContent(
    Control Root,
    Func<IReadOnlyDictionary<string, object?>> CaptureState,
    IDisposable? OwnedState = null);
