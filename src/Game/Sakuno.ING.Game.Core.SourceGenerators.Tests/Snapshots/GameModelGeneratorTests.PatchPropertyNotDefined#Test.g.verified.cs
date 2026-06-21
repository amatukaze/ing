//HintName: Test.g.cs
#nullable disable
using TestNamespace;

namespace Sakuno.ING.Game.Models;
public sealed partial class Test : BindableObject, IModel<Test, TestId, ITestUpdated>, IPatchable<TestId, ITestPatched>
{
    public TestId Id { get; }
    public int A { get; private set => SetField(ref field, value, PropertyNames.A); }

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
        UpdateCore(raw);
    }

    public void Patch(ITestPatched patch)
    {
        switch (patch)
        {
        }
    }

    public void Patch<TPatch>(TPatch patch)
        where TPatch : ITestPatched
    {
        Patch((ITestPatched)patch);
    }

    partial void CreateCore();
    partial void UpdateCore(ITestUpdated raw);
}