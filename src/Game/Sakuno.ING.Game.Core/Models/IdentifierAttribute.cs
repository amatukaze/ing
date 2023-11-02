namespace Sakuno.ING.Game.Models;

[AttributeUsage(AttributeTargets.Struct)]
public sealed class IdentifierAttribute : Attribute
{
    public bool NoToString { get; set; }
}
