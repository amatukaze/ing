using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class AirBase : ModelBase
    {
        public IDictionary<int, IDTable<AirForceGroup>> Table { get; } = new ListDictionary<int, IDTable<AirForceGroup>>();

        public IEnumerable<AirForceGroup> AllGroups => Table.Values.SelectMany(r => r.Values);

        internal AirBase()
        {
            ApiService.Subscribe("api_req_air_corps/set_action", r =>
            {
                var rAreaID = int.Parse(r.Parameters["api_area_id"]);
                var rGroups = r.Parameters["api_base_id"].Split(',').Select(rpID => Table[rAreaID][int.Parse(rpID)]).ToArray();
                var rOptions = r.Parameters["api_action_kind"].Split(',').Select(rpOption => (AirForceGroupOption)int.Parse(rpOption)).ToArray();

                for (int i = 0; i < rGroups.Length; i++)
                    rGroups[i].Option = rOptions[i];
            });

            ApiService.Subscribe("api_req_air_corps/change_name", r =>
            {
                var rAreaID = int.Parse(r.Parameters["api_area_id"]);
                var rGroup = Table[rAreaID][int.Parse(r.Parameters["api_base_id"])];
                rGroup.Name = r.Parameters["api_name"];
            });

            ApiService.Subscribe("api_req_air_corps/set_plane", r =>
            {
                var rAreaID = int.Parse(r.Parameters["api_area_id"]);
                var rGroup = Table[rAreaID][int.Parse(r.Parameters["api_base_id"])];

                var rData = r.GetData<RawAirForceGroupOrganization>();
                foreach (var rSquadron in rData.Squadrons)
                    rGroup.Squadrons[rSquadron.ID].Update(rSquadron);

                rGroup.CombatRadius = rData.CombatRadius;
                rGroup.UpdateFighterPower();
                rGroup.UpdateLBASConsumption();
            });

            ApiService.Subscribe("api_req_air_corps/supply", r =>
            {
                var rAreaID = int.Parse(r.Parameters["api_area_id"]);
                var rGroup = Table[rAreaID][int.Parse(r.Parameters["api_base_id"])];

                var rData = r.GetData<RawAirForceSquadronResupplyResult>();
                foreach (var rSquadron in rData.Squadrons)
                    rGroup.Squadrons[rSquadron.ID].Update(rSquadron);

                rGroup.UpdateFighterPower();
                rGroup.UpdateLBASConsumption();
            });

            ApiService.Subscribe("api_req_air_corps/expand_base", r =>
            {
                var rRawGroups = r.GetData<RawAirForceGroup[]>();

                foreach (var rRawGroup in rRawGroups)
                {
                    IDTable<AirForceGroup> rGroups;
                    if (!Table.TryGetValue(rRawGroup.AreaID, out rGroups))
                        Table.Add(rRawGroup.AreaID, rGroups = new Game.IDTable<Models.AirForceGroup>());

                    AirForceGroup rGroup;
                    if (!rGroups.TryGetValue(rRawGroup.ID, out rGroup))
                        rGroups.Add(new AirForceGroup(rRawGroup));
                    else
                        rGroup.Update(rRawGroup);
                }
            });
        }

        internal void UpdateGroups(RawAirForceGroup[] rpGroups)
        {
            if (rpGroups == null)
                return;

            HashSet<int> rRemovedIDs = null;
            if (Table.Count > 0)
                rRemovedIDs = new HashSet<int>(Table.Keys);

            var rUpdate = false;

            var rAreas = rpGroups.GroupBy(r => r.AreaID);
            foreach (var rArea in rAreas)
            {
                var rAreaID = rArea.Key;

                IDTable<AirForceGroup> rGroups;
                if (!Table.TryGetValue(rAreaID, out rGroups))
                    Table.Add(rAreaID, rGroups = new IDTable<AirForceGroup>());

                rUpdate |= rGroups.UpdateRawData(rArea, r => new AirForceGroup(r), (rpData, rpRawData) => rpData.Update(rpRawData));

                if (rRemovedIDs != null)
                    rRemovedIDs.Remove(rAreaID);
            }

            if (rRemovedIDs != null)
                foreach (var rID in rRemovedIDs)
                {
                    Table.Remove(rID);
                    rUpdate = true;
                }

            if (rUpdate)
                OnPropertyChanged(nameof(AllGroups));
        }
    }
}
