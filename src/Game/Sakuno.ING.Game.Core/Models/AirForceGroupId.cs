using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models;

public readonly record struct AirForceGroupId(MapAreaId AreaId, int Index) : IIdentifier<AirForceGroupId, (MapAreaId, int)>, IComparable<AirForceGroupId>
{
    public static AirForceGroupId From((MapAreaId, int) value)
    {
        var (areaId, index) = value;

        return new(areaId, index);
    }

    public int CompareTo(AirForceGroupId other)
    {
        var result = AreaId.CompareTo(other.AreaId);

        return result is 0 ? Index.CompareTo(other.Index) : result;
    }
}
