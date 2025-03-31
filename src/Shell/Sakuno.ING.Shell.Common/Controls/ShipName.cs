using Microsoft.Extensions.DependencyInjection;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Game.Services;

namespace Sakuno.ING.Shell.Controls;

[TemplatePart("PART_TextBlock", typeof(TextBlock), IsRequired = true)]
public class ShipName : TemplatedControl
{
    public static readonly StyledProperty<ShipInfoId> ShipProperty =
        AvaloniaProperty.Register<ShipName, ShipInfoId>(nameof(Ship));

    public ShipInfoId Ship
    {
        get => GetValue(ShipProperty);
        set => SetValue(ShipProperty, value);
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

        if (e.Property == ShipProperty)
            UpdateName();
    }

    private void UpdateName()
    {
        if (_textBlock is null)
            return;

        var ships = DependencyInjection.GetContainer(this).GetRequiredService<MasterDataService>().Ships;

        _textBlock.Text = ships.TryGetValue(Ship, out var shipInfo) ? shipInfo.Name : "?";
    }
}
