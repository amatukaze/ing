using System.Reactive;
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

        fullUpdateSubject.OnNext([
            (1, 2),
            (2, 3),
            (7, 4),
            (9, 5),
            (10, 6),
        ]);
        fullUpdateSubject.OnNext([
            (1, 2),
            (2, 3),
            (4, 1),
            (9, 5),
            (10, 6),
            (11, 7),
        ]);

        Assert.Equal(6, table.Count);
        Assert.Equal([
            (1, 2),
            (2, 3),
            (4, 1),
            (9, 5),
            (10, 6),
            (11, 7),
        ], table);
    }

    [Fact]
    public void PartialUpdate()
    {
        var fullUpdateSubject = new Subject<RawTestItem[]>();
        var partialUpdateSubject = new Subject<RawTestItem[]>();
        var committingSubject = new Subject<Unit>();

        var table = new Table<TestItem, int, RawTestItem>(fullUpdateSubject, partialUpdateSubject,  committingSource: committingSubject);

        Assert.Empty(table);

        fullUpdateSubject.OnNext([
            (1, 2),
            (2, 3),
            (7, 4),
            (9, 5),
            (10, 6),
        ]);

        Assert.Equal(5, table.Count);

        partialUpdateSubject.OnNext([
            (2, 30),
            (7, 40),
            (5, 1),
            (13, 401),
        ]);

        Assert.Equal(5, table.Count);

        committingSubject.OnNext(Unit.Default);

        Assert.Equal(7, table.Count);
        Assert.Equal([
            (1, 2),
            (2, 30),
            (5, 1),
            (7, 40),
            (9, 5),
            (10, 6),
            (13, 401),
        ], table);
    }

    [Fact]
    public void Remove()
    {
        var fullUpdateSubject = new Subject<RawTestItem[]>();
        var removeSubject = new Subject<int[]>();
        var committingSubject = new Subject<Unit>();

        var table = new Table<TestItem, int, RawTestItem>(fullUpdateSubject, removeSource: removeSubject, committingSource: committingSubject);

        Assert.Empty(table);

        fullUpdateSubject.OnNext([
            (1, 2),
            (2, 3),
            (7, 4),
            (9, 5),
            (10, 6),
        ]);

        Assert.Equal(5, table.Count);

        removeSubject.OnNext([2, 10, 111]);

        Assert.Equal(5, table.Count);

        committingSubject.OnNext(Unit.Default);

        Assert.Equal(3, table.Count);
        Assert.Equal([
            (1, 2),
            (7, 4),
            (9, 5),
        ], table);
    }

    [Fact]
    public void CommittingSourceShouldNotBeNullWithPartialUpdatedOrRemoved()
    {
        var fullUpdateSubject = new Subject<RawTestItem[]>();
        var partialUpdateSubject = new Subject<RawTestItem[]>();
        var removeSubject = new Subject<int[]>();

        Assert.Throws<ArgumentNullException>(() =>
        {
            _ = new Table<TestItem, int, RawTestItem>(fullUpdateSubject, partialUpdateSource: partialUpdateSubject);
        });
        Assert.Throws<ArgumentNullException>(() =>
        {
            _ = new Table<TestItem, int, RawTestItem>(fullUpdateSubject, removeSource: removeSubject);
        });
        Assert.Throws<ArgumentNullException>(() =>
        {
            _ = new Table<TestItem, int, RawTestItem>(fullUpdateSubject, partialUpdateSubject, removeSubject);
        });
    }
}
