namespace Sakuno.ING.Game.Provider;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public sealed class ApiAttribute(string api) : Attribute
{
    public string Api { get; } = api;
}
