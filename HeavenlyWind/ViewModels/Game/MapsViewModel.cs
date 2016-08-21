using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Views.Game;
using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    [ViewInfo(typeof(Maps))]
    public class MapsViewModel : TabItemViewModel
    {
        public override string Name
        {
            get { return StringResources.Instance.Main.Tab_Maps; }
            protected set { throw new NotImplementedException(); }
        }

        public MapGaugesViewModel MapGauges { get; } = new MapGaugesViewModel();

        public EventMapOverviewViewModel EventMaps { get; private set; }

        public MapsViewModel()
        {
            SessionService.Instance.SubscribeOnce("api_get_member/require_info", delegate
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
