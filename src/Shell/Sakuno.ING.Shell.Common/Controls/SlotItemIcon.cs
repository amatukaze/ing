using Microsoft.Extensions.DependencyInjection;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Game.Services;

namespace Sakuno.ING.Shell.Controls;

[TemplatePart("PART_TextBlock", typeof(TextBlock), IsRequired = true)]
public class SlotItemIcon : TemplatedControl
{
    public static readonly StyledProperty<SlotItemInfoId> SlotItemProperty =
        AvaloniaProperty.Register<SlotItemIcon, SlotItemInfoId>(nameof(SlotItem));

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

        UpdateIcon();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Property == SlotItemProperty)
            UpdateIcon();
    }

    private void UpdateIcon()
    {
        if (_textBlock is null)
            return;

        var id = SlotItem;
        if (id.IsValid)
        {
            var slotItems = DependencyInjection.GetContainer(this).GetRequiredService<MasterDataService>().SlotItems;
            if (slotItems.Snapshot.TryGetValue(id, out var slotItemInfo))
            {
                _textBlock.Text = slotItemInfo.IconId.ToString();
                return;
            }

            _textBlock.Text = "?";
            return;
        }

        _textBlock.Text = string.Empty;
    }
}
