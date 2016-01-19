namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    class PastEventMapInfo : IMapMasterInfo
    {
        public int ID { get; }
        public int AreaID { get; }
        public int AreaSubID { get; }

        public string Name => "?????";

        public PastEventMapInfo(int rpMapID)
        {
            ID = rpMapID;
            AreaID = rpMapID / 10;
            AreaSubID = rpMapID % 10;
        }
    }
}
