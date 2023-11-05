namespace Sakuno.ING.Game.Tests;

internal record TestItem : IModel<TestItem, int, RawTestItem>
{
    public int Id { get; }
    public int Value { get; private set; }

    public TestItem(int id, int value)
    {
        Id = id;
        Value = value;
    }

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
