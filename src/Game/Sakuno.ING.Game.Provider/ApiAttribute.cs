namespace Sakuno.ING.Game.Provider;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
internal sealed class ApiAttribute : Attribute
{
    public string Api { get; }

    public ApiAttribute(string api)
    {
        Api = api;
    }
}
