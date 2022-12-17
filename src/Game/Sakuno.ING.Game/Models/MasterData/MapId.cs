namespace Sakuno.ING.Game.Models.MasterData;

[Identifier]
public readonly struct MapId : IEquatable<MapId>, IComparable<MapId>
{
    private readonly int _value;

    public MapAreaId AreaId => (MapAreaId)(_value / 10);
    public int CategoryNo => _value % 10;

    public MapId(int value) => _value = value;

    public int CompareTo(MapId other) => _value - other._value;
    public bool Equals(MapId other) => _value == other._value;

    public static bool operator ==(MapId left, MapId right) => left._value == right._value;
    public static bool operator !=(MapId left, MapId right) => left._value != right._value;
    public static implicit operator int(MapId id) => id._value;
    public static explicit operator MapId(int value) => new(value);

    public override bool Equals(object? obj) => obj is MapId other && other == this;
    public override int GetHashCode() => _value;
    public override string ToString()
    {
        var area = Math.DivRem(_value, 10, out var category);

        return $"{area}-{category}";
    }
}
