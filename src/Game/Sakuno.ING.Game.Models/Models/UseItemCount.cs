namespace Sakuno.ING.Game.Models
{
    public partial class UseItemCount
    {
        partial void CreateDummy() => Item = owner.MasterData.UseItems[Id];
    }
}
