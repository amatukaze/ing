namespace Sakuno.ING.Game.Tests;

internal record RawTestItem(int Id, int Value) : IIdentifiable<int>
{
    public static implicit operator RawTestItem((int, int) tuple)
    {
        var (id, value) = tuple;

        return new(id, value);
    }
}
