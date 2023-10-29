namespace Sakuno.ING.Game.Models.MasterData;

[Identifier(NoToString = true)]
public readonly partial struct MapId
{
    public MapAreaId AreaId => (MapAreaId)(_value / 10);
    public int CategoryNo => _value % 10;

    public override string ToString()
    {
        var area = Math.DivRem(_value, 10, out var category);

        return $"{area}-{category}";
    }
}
