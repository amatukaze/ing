using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class MapGaugesViewModel : ModelBase
    {
        public IList<MapInfo> EventMaps { get; private set; }
        public IList<MapInfo> ExtraOperations { get; private set; }

        internal MapGaugesViewModel()
        {
            SessionService.Instance.Subscribe("api_get_member/mapinfo", _ => Process());
        }

        void Process()
        {
            var rMapWithGauge = KanColleGame.Current.Maps.Values.Where(r => !r.IsCleared).ToLookup(r => r.IsEventMap);

            EventMaps = !rMapWithGauge[true].Any() ? null : rMapWithGauge[true].ToList().AsReadOnly();
            ExtraOperations = rMapWithGauge[false].ToList().AsReadOnly();

            OnPropertyChanged(nameof(EventMaps));
            OnPropertyChanged(nameof(ExtraOperations));
        }
    }
}
