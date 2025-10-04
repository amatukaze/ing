namespace Sakuno.ING.Game.Provider;

[AttributeUsage(AttributeTargets.Parameter)]
public class FromRequestAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
