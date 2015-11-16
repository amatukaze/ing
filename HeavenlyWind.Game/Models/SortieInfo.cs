using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Collections.Generic;
using System;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class SortieInfo : ModelBase
    {
        static SortieInfo r_Current;

        public Fleet Fleet { get; }
        public MapMasterInfo Map { get; }

        SortieCellInfo r_Cell;
        public SortieCellInfo Cell
        {
            get { return r_Cell; }
            private set
            {
                if (r_Cell != value)
                {
                    r_Cell = value;
                    OnPropertyChanged(nameof(Cell));
                }
            }
        }

        static SortieInfo()
        {
            SessionService.Instance.Subscribe("api_port/port", _ => r_Current = null);

            Action<ApiData> rExplorationParser = r => r_Current?.Explore(r.Requests, r.GetData<RawMapExploration>());
            SessionService.Instance.Subscribe("api_req_map/start", rExplorationParser);
            SessionService.Instance.Subscribe("api_req_map/next", rExplorationParser);
        }
        internal SortieInfo(Fleet rpFleet, int rpMapID)
        {
            r_Current = this;

            Fleet = rpFleet;
            Map = KanColleGame.Current.MasterInfo.Maps[rpMapID];
        }

        void Explore(IReadOnlyDictionary<string, string> rpRequests, RawMapExploration rpData)
        {
            Cell = new SortieCellInfo(rpData);
        }
    }
}
