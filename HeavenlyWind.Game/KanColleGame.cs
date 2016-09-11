using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;

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

        public event Action<SortieInfo> ReturnedFromSortie = delegate { };

        KanColleGame()
        {
            ApiService.Subscribe("api_get_member/mapinfo", rpApiData =>
            {
                if (Maps.UpdateRawData(rpApiData.GetData<RawMapInfo[]>(), r => new MapInfo(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
                    OnPropertyChanged(nameof(Maps));
            });
            ApiService.Subscribe("api_req_map/select_eventmap_rank", r =>
            {
                var rMap = Maps[int.Parse(r.Parameters["api_maparea_id"]) * 10 + int.Parse(r.Parameters["api_map_no"])];
                rMap.Difficulty = (EventMapDifficulty)int.Parse(r.Parameters["api_rank"]);
                rMap.HP.Set(9999, 9999);
            });

            ApiService.Subscribe(new[] { "api_req_sortie/battleresult", "api_req_combined_battle/battleresult" }, r =>
            {
                var rSortieMap = Sortie.Map;
                if (!rSortieMap.HasGauge || Sortie.Node.EventType != SortieEventType.BossBattle)
                    return;

                var rBattle = BattleInfo.Current;
                var rEnemyFlagship = rBattle.CurrentStage.Enemy[0];
                if (!rSortieMap.IsEventMap && rEnemyFlagship.State == BattleParticipantState.Sunk)
                    rSortieMap.HP.Current--;
                else if (rSortieMap.IsEventMap)
                {
                    var rData = r.GetData<RawBattleResult>();
                    if (rData.TransportMissionResult != null)
                        rSortieMap.HP.Current -= rData.TransportMissionResult.Point;
                    else
                    {
                        var rCurrentHP = rSortieMap.HP.Current - (rEnemyFlagship.Maximum - Math.Max(rEnemyFlagship.Current, 0));
                        if (rEnemyFlagship.State == BattleParticipantState.Sunk)
                            rSortieMap.HP.Current = Math.Max(rCurrentHP, 0);
                        else
                            rSortieMap.HP.Current = Math.Max(rCurrentHP, 1);
                    }
                }

                rSortieMap.UpdateGauge();
            });
            ApiService.Subscribe("api_req_map/next", r =>
            {
                var rSortieMap = Sortie.Map;
                if (rSortieMap.IsCleared || ((RawMapExploration)r.Data).NodeEventType != SortieEventType.EscortSuccess)
                    return;

                rSortieMap.HP.Current--;

                rSortieMap.UpdateGauge();
            });
        }

        internal void RaiseReturnedFromSortie(SortieInfo rpSortie) => ReturnedFromSortie(rpSortie);
    }
}
