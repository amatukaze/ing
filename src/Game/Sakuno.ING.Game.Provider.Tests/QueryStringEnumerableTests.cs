using Sakuno.ING.Game.Models;
using Xunit;

namespace Sakuno.ING.Game.Provider.Tests;

public class QueryStringEnumerableTests
{
    [Fact]
    public void SimpleQueryString()
    {
        var input = "a=1&bb=str"u8;

        var enumerator = input.EnumerateQueryString().GetEnumerator();

        Assert.True(enumerator.MoveNext());
        Assert.Equal("a"u8, enumerator.Current.Name);
        Assert.Equal("1"u8, enumerator.Current.Value);
        Assert.Equal(1, enumerator.Current.DecodeValueAsInt());
        Assert.Equal((ShipId)1, enumerator.Current.DecodeValueAsIdentifier<ShipId>());

        Assert.True(enumerator.MoveNext());
        Assert.Equal("bb"u8, enumerator.Current.Name);
        Assert.Equal("str"u8, enumerator.Current.Value);

        Assert.False(enumerator.MoveNext());
    }

    [Fact]
    public void EscapedQueryString()
    {
        var input = "name=%E8%89%A6%E3%81%93%E3%82%8C+KanColle"u8;

        var enumerator = input.EnumerateQueryString().GetEnumerator();

        Assert.True(enumerator.MoveNext());
        Assert.Equal("name"u8, enumerator.Current.Name);
        Assert.Equal("艦これ KanColle", enumerator.Current.DecodeValueAsString());

        Assert.False(enumerator.MoveNext());
    }
}
