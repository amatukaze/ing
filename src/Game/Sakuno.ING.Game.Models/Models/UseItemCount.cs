namespace Sakuno.ING.Game.Models
{
    public partial class UseItemCount
    {
        partial void CreateCore() => UseItem = _owner.MasterData.UseItems[Id];
    }
}
