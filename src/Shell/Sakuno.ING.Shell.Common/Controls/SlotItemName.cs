using Microsoft.Extensions.DependencyInjection;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Game.Services;

namespace Sakuno.ING.Shell.Controls;

[TemplatePart("PART_TextBlock", typeof(TextBlock), IsRequired = true)]
public class SlotItemName : TemplatedControl
{
    public static readonly StyledProperty<SlotItemInfoId> SlotItemProperty =
        AvaloniaProperty.Register<SlotItemName, SlotItemInfoId>(nameof(SlotItem));

    public SlotItemInfoId SlotItem
    {
        get => GetValue(SlotItemProperty);
        set => SetValue(SlotItemProperty, value);
    }

    private TextBlock? _textBlock;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _textBlock = e.NameScope.Find<TextBlock>("PART_TextBlock");

        UpdateName();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Property == SlotItemProperty)
            UpdateName();
    }

    private void UpdateName()
    {
        if (_textBlock is null)
            return;

        var slotItems = DependencyInjection.GetContainer(this).GetRequiredService<MasterDataService>().SlotItems;

        _textBlock.Text = slotItems.TryGetValue(SlotItem, out var slotItemInfo) ? slotItemInfo.Name : "?";
    }
}
