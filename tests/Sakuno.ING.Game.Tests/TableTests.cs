using System.Reactive.Subjects;

namespace Sakuno.ING.Game.Tests;

public class TableTests
{
    [Fact]
    public void FullUpdate()
    {
        var fullUpdateSubject = new Subject<RawTestItem[]>();

        var table = new Table<TestItem, int, RawTestItem>(fullUpdateSubject);

        Assert.Empty(table);

        fullUpdateSubject.OnNext(new RawTestItem[]
        {
            (1, 2),
            (2, 3),
            (7, 4),
            (9, 5),
            (10, 6),
        });
        fullUpdateSubject.OnNext(new RawTestItem[]
        {
            (1, 2),
            (2, 3),
            (4, 1),
            (9, 5),
            (10, 6),
            (11, 7),
        });

        Assert.Equal(6, table.Count);
        Assert.Equal(new TestItem[]
        {
            (1, 2),
            (2, 3),
            (4, 1),
            (9, 5),
            (10, 6),
            (11, 7),
        }, table);
    }

    [Fact]
    public void PartialUpdate()
    {
        var fullUpdateSubject = new Subject<RawTestItem[]>();
        var partialUpdateSubject = new Subject<RawTestItem[]>();

        var table = new Table<TestItem, int, RawTestItem>(fullUpdateSubject, partialUpdateSubject);

        Assert.Empty(table);

        fullUpdateSubject.OnNext(new RawTestItem[]
        {
            (1, 2),
            (2, 3),
            (7, 4),
            (9, 5),
            (10, 6),
        });
        partialUpdateSubject.OnNext(new RawTestItem[]
        {
            (2, 30),
            (7, 40),
            (5, 1),
            (13, 401),
        });

        Assert.Equal(7, table.Count);
        Assert.Equal(new TestItem[]
        {
            (1, 2),
            (2, 30),
            (5, 1),
            (7, 40),
            (9, 5),
            (10, 6),
            (13, 401),
        }, table);
    }

    [Fact]
    public void Remove()
    {
        var fullUpdateSubject = new Subject<RawTestItem[]>();
        var removeSubject = new Subject<int[]>();

        var table = new Table<TestItem, int, RawTestItem>(fullUpdateSubject, removeSource: removeSubject);

        Assert.Empty(table);

        fullUpdateSubject.OnNext(new RawTestItem[]
        {
            (1, 2),
            (2, 3),
            (7, 4),
            (9, 5),
            (10, 6),
        });
        removeSubject.OnNext(new[] { 2, 10, 111 });

        Assert.Equal(3, table.Count);
        Assert.Equal(new TestItem[]
        {
            (1, 2),
            (7, 4),
            (9, 5),
        }, table);
    }
}
