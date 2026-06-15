using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.ViewModels.Tests.Homeport;

public class SlotItemCountTests
{
    private readonly Subject<IReadOnlyList<ISlotItemUpdated>> _slotItemsUpdatedSubject = new();
    private readonly Subject<IAdmiralUpdated> _admiralUpdatedSubject = new();

    private readonly ObservableCollector<int> _count = new();
    private readonly ObservableCollector<int> _maxCount = new();

    public SlotItemCountTests()
    {
        var provider = Substitute.For<IGameProvider>();
        provider.SlotItemsUpdated.Returns(_slotItemsUpdatedSubject);
        provider.AdmiralUpdated.Returns(_admiralUpdatedSubject);

        var vm = new SlotItemCountViewModel(new PlayerDataService(provider));
        vm.Count.Subscribe(_count);
        vm.MaxCount.Subscribe(_maxCount);
    }

    [Fact]
    public void CountAndMaxCountStartAtZero()
    {
        Assert.Equal(0, _count.LatestValue);
        Assert.Equal(0, _maxCount.LatestValue);
    }

    [Fact]
    public void CountExcludesItemsByMasterId()
    {
        _slotItemsUpdatedSubject.OnNext(
        [
            CreateSlotItem(1, 1),
            CreateSlotItem(2, 42),   // excluded
            CreateSlotItem(3, 43),   // excluded
            CreateSlotItem(4, 100),
            CreateSlotItem(5, 241),  // excluded
        ]);

        Assert.Equal(2, _count.LatestValue);
    }

    [Fact]
    public void MaxCountSetAfterAdmiralUpdatedEvent()
    {
        var admiral = Substitute.For<IAdmiralUpdated>();
        admiral.MaxSlotItemCount.Returns(550);

        _admiralUpdatedSubject.OnNext(admiral);

        Assert.Equal(550, _maxCount.LatestValue);
    }

    private static ISlotItemUpdated CreateSlotItem(int id, int masterId)
    {
        var item = Utils.GenerateMock<ISlotItemUpdated, SlotItemId>(id);
        item.MasterId.Returns(SlotItemInfoId.From(masterId));
        return item;
    }
}
