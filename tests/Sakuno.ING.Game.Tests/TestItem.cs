namespace Sakuno.ING.Game.Tests;

internal record TestItem(int Id, int Value) : IModel<TestItem, int, RawTestItem>
{
    public int Value { get; private set; } = Value;

    public static TestItem Create(RawTestItem raw)
    {
        var (id, value) = raw;

        return new(id, value);
    }

    public void Update(RawTestItem raw) =>
        (_, Value) = raw;

    public static implicit operator TestItem((int, int) tuple)
    {
        var (id, value) = tuple;

        return new(id, value);
    }
}
