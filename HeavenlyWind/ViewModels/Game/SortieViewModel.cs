using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
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
        IDisposable r_EventMapShipLockingSubscription;

        public SortieInfo Info { get; private set; }

        internal SortieViewModel()
        {
            var rGame = KanColleGame.Current;

            PropertyChangedEventListener.FromSource(rGame).Add(nameof(rGame.Sortie), delegate
            {
                var rInfo = rGame.Sortie;
                if (rInfo == null)
                    Type = DisplayType.MapGauge;
                else
                {
                    Info = rInfo;
                    OnPropertyChanged(nameof(Info));

                    Type = rInfo is PracticeInfo ? DisplayType.Practice : DisplayType.Sortie;
                }
            });

            r_EventMapShipLockingSubscription = SessionService.Instance.Subscribe("api_get_member/basic", delegate
            {
                ShipLockingService.Instance.Initialize();

                var rMasterInfo = rGame.MasterInfo;
                if (ShipLockingService.Instance.ShipLocking.Count > 0 && rMasterInfo.EventMapCount > 0)
                {
                    var rEventMaps = from rArea in rMasterInfo.MapAreas.Values
                                     where rArea.IsEventArea
                                     join rMap in rMasterInfo.Maps.Values on rArea.ID equals rMap.AreaID
                                     select rMap;

                    EventMaps = new EventMapOverviewViewModel(rEventMaps.ToArray());
                    OnPropertyChanged(nameof(EventMaps));
                }

                r_EventMapShipLockingSubscription.Dispose();
                r_EventMapShipLockingSubscription = null;
            });
        }
    }
}
