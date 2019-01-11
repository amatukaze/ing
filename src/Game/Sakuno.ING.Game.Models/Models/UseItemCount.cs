namespace Sakuno.ING.Game.Models
{
    public partial class UseItemCount
    {
        partial void CreateCore() => Item = owner.MasterData.UseItems[Id];
    }
}
