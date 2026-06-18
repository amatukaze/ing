using Microsoft.Extensions.DependencyInjection;
using Sakuno.ING.Game;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Shell.Controls;

[TemplatePart("PART_TextBlock", typeof(TextBlock), IsRequired = true)]
public class MapAreaName : TemplatedControl
{
    public static readonly StyledProperty<MapAreaId> MapAreaProperty =
        AvaloniaProperty.Register<MapAreaName, MapAreaId>(nameof(MapArea));

    public MapAreaId MapArea
    {
        get => GetValue(MapAreaProperty);
        set => SetValue(MapAreaProperty, value);
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

        if (e.Property == MapAreaProperty)
            UpdateName();
    }

    private void UpdateName()
    {
        if (_textBlock is null)
            return;

        var id = MapArea;
        if (id.IsValid)
        {
            var mapAreas = DependencyInjection.GetContainer(this).GetRequiredService<IMasterDataService>().MapAreas;
            if (mapAreas.Snapshot.TryGetValue(id, out var mapAreaInfo))
            {
                _textBlock.Text = mapAreaInfo.Name;
                return;
            }
        }

        _textBlock.Text =  "?";
    }
}
