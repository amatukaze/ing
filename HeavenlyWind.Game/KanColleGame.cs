using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class KanColleGame : ModelBase
    {
        public static KanColleGame Current { get; } = new KanColleGame();

        public MasterInfo MasterInfo { get; } = new MasterInfo();

        public Port Port { get; } = new Port();

        bool r_IsStarted;
        public bool IsStarted
        {
            get { return r_IsStarted; }
            internal set
            {
                if (r_IsStarted != value)
                {
                    r_IsStarted = value;
                    OnPropertyChanged(nameof(IsStarted));
                }
            }
        }

        public IDTable<MapInfo> Maps { get; } = new IDTable<MapInfo>();

        SortieInfo r_Sortie;
        public SortieInfo Sortie
        {
            get { return r_Sortie; }
            internal set
            {
                if (r_Sortie != value)
                {
                    r_Sortie = value;
                    OnPropertyChanged(nameof(Sortie));
                }
            }
        }
        internal SortieInfo OldSortie { get; set; }

        KanColleGame()
        {
            SessionService.Instance.Subscribe("api_get_member/mapinfo", rpApiData =>
            {
                if (Maps.UpdateRawData(rpApiData.GetData<RawMapInfo[]>(), r => new MapInfo(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
                    OnPropertyChanged(nameof(Maps));
            });
            SessionService.Instance.Subscribe("api_req_map/select_eventmap_rank", r =>
            {
                var rMap = Maps[int.Parse(r.Requests["api_maparea_id"]) * 10 + int.Parse(r.Requests["api_map_no"])];
                rMap.Difficulty = (EventMapDifficulty)int.Parse(r.Requests["api_rank"]);
            });

            SessionService.Instance.Subscribe(new[] { "api_req_sortie/battleresult", "api_req_combined_battle/battleresult" }, _ =>
            {
                var rSortieMap = Sortie.Map;
                if (rSortieMap.IsCleared || rSortieMap.IsEventMap || Sortie.Node.EventType != SortieEventType.BossBattle)
                    return;

                var rBattle = BattleInfo.Current;
                if (rBattle.CurrentStage.Enemy[0].State == BattleParticipantState.Sunk)
                    rSortieMap.HP = rSortieMap.HP - 1;
            });
            SessionService.Instance.Subscribe("api_req_map/next", r =>
            {
                var rSortieMap = Sortie.Map;
                if (rSortieMap.IsCleared || ((RawMapExploration)r.Data).NodeEventType != SortieEventType.EscortSuccess)
                    return;

                rSortieMap.HP = rSortieMap.HP - 1;
            });
        }
    }
}
