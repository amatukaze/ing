//HintName: Test.g.cs
#nullable disable
using TestNamespace;

namespace Sakuno.ING.Game.Models;
public sealed partial class Test : BindableObject, IModel<Test, TestId, ITestUpdated>
{
    public TestId Id { get; }
    public int A { get; private set => SetField(ref field, value, PropertyNames.A); }
    public int B { get; private set => SetField(ref field, value, PropertyNames.B); }

    public Test(TestId id)
    {
        Id = id;
        CreateCore();
    }

    public Test(ITestUpdated raw) : this(raw.Id)
    {
        Update(raw);
    }

    public static Test Create(ITestUpdated raw) => new(raw);
    public void Update(ITestUpdated raw)
    {
        A = raw.A;
        B = raw.B;
        UpdateCore(raw);
    }

    partial void CreateCore();
    partial void UpdateCore(ITestUpdated raw);
}
