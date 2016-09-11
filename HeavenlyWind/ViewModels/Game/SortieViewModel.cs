using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Views.Game;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    [ViewInfo(typeof(SortieOverview))]
    public class SortieViewModel : TabItemViewModel
    {
        public enum DisplayType { MapGauge, Sortie, Practice }

        public override string Name
        {
            get { return StringResources.Instance.Main.Tab_Sortie; }
            protected set { throw new NotImplementedException(); }
        }

        DisplayType r_Type;
        public DisplayType Type
        {
            get { return r_Type; }
            private set
            {
                r_Type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public MapGaugesViewModel MapGauges { get; } = new MapGaugesViewModel();

        public EventMapOverviewViewModel EventMaps { get; private set; }

        SortieInfo r_Info;
        public SortieInfo Info
        {
            get { return r_Info; }
            private set
            {
                if (r_Info != value)
                {
                    r_Info = value;
                    OnPropertyChanged(nameof(Info));
                }
            }
        }

        internal SortieViewModel()
        {
            ApiService.Subscribe("api_req_map/start", delegate
            {
                Info = SortieInfo.Current;
                Type = DisplayType.Sortie;
            });

            ApiService.Subscribe("api_req_member/get_practice_enemyinfo", delegate
            {
                Info = KanColleGame.Current.Sortie;
                Type = DisplayType.Practice;
            });

            ApiService.Subscribe("api_port/port", _ =>
            {
                Info = null;
                Type = DisplayType.MapGauge;
            });

            ApiService.SubscribeOnce("api_get_member/require_info", delegate
            {
                ShipLockingService.Instance.Initialize();

                var rMasterInfo = KanColleGame.Current.MasterInfo;
                if (ShipLockingService.Instance.ShipLocking.Count > 0 && rMasterInfo.EventMapCount > 0)
                {
                    var rEventMaps = from rArea in rMasterInfo.MapAreas.Values
                                     where rArea.IsEventArea
                                     join rMap in rMasterInfo.Maps.Values on rArea.ID equals rMap.AreaID
                                     select rMap;

                    EventMaps = new EventMapOverviewViewModel(rEventMaps.ToArray());
                    OnPropertyChanged(nameof(EventMaps));
                }
            });
        }
    }
}
