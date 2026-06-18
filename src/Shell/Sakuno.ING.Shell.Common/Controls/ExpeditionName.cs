using Microsoft.Extensions.DependencyInjection;
using Sakuno.ING.Game;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Shell.Controls;

[TemplatePart("PART_TextBlock", typeof(TextBlock), IsRequired = true)]
public class ExpeditionName : TemplatedControl
{
    public static readonly StyledProperty<ExpeditionId> ExpeditionProperty =
        AvaloniaProperty.Register<ExpeditionName, ExpeditionId>(nameof(Expedition));

    public ExpeditionId Expedition
    {
        get => GetValue(ExpeditionProperty);
        set => SetValue(ExpeditionProperty, value);
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

        if (e.Property == ExpeditionProperty)
            UpdateName();
    }

    private void UpdateName()
    {
        if (_textBlock is null)
            return;

        var id = Expedition;
        if (id.IsValid)
        {
            var expeditions = DependencyInjection.GetContainer(this).GetRequiredService<IMasterDataService>().Expeditions;
            if (expeditions.Snapshot.TryGetValue(id, out var expeditionInfo))
            {
                _textBlock.Text = expeditionInfo.Name;
                return;
            }
        }

        _textBlock.Text =  "?";
    }
}
