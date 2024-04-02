namespace Sakuno.ING.Composition;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class ExportAttribute : Attribute
{
    public Type? ContractType { get; }
    public bool Singleton { get; set; } = true;
    public bool LazyCreate { get; set; } = true;

    public ExportAttribute() { }
    public ExportAttribute(Type contractType) => ContractType = contractType;
}
