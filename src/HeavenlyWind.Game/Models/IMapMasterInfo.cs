namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public interface IMapMasterInfo
    {
        int ID { get; }
        int AreaID { get; }
        int AreaSubID { get; }

        string Name { get; }
    }
}
