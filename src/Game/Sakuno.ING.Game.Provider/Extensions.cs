namespace Sakuno.ING.Game.Provider;

internal static class Extensions
{
    public static QueryStringEnumerable EnumerateQueryString(this ReadOnlySpan<byte> source) => new(source);
}
