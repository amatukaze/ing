using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Records
{
    class SortieMapFilterKey : ModelBase
    {
        public static SortieMapFilterKey All { get; } = new SortieMapFilterKey();

        public IMapMasterInfo Map { get; }
        public bool IsEventMap { get; }
        public EventMapDifficulty EventMapDifficulty { get; }

        SortieMapFilterKey() { }
        public SortieMapFilterKey(IMapMasterInfo rpMap, EventMapDifficulty rpDifficulty = EventMapDifficulty.None)
        {
            Map = rpMap;
            IsEventMap = rpDifficulty != EventMapDifficulty.None;
            EventMapDifficulty = rpDifficulty;
        }

        internal static int Comparer(SortieMapFilterKey x, SortieMapFilterKey y)
        {
            var rResult = x.Map.ID - y.Map.ID;

            if (rResult == 0 && x.IsEventMap)
                rResult = y.EventMapDifficulty - x.EventMapDifficulty;

            return rResult;
        }
    }
}
