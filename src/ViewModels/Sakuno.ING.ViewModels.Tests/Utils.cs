using Sakuno.ING.Game;

namespace Sakuno.ING.ViewModels.Tests;

public static class Utils
{
    public static T GenerateMock<T, TId>(int id)
        where T : class, IIdentifiable<TId>
        where TId : IIdentifier<TId, int>
    {
        var result = Substitute.For<T>();

        result.Id.Returns(TId.From(id));

        return result;
    }

    public static IEnumerable<T> GenerateMocks<T, TId>(int fromId, int count)
        where T : class, IIdentifiable<TId>
        where TId : IIdentifier<TId, int>
    {
        for (var i = fromId; i < fromId + count; i++)
            yield return GenerateMock<T, TId>(i);
    }
}
