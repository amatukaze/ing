using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class AirBase : ModelBase
    {
        public IDTable<AirForceGroup> Table { get; private set; }

        public AirForceGroup this[int rpID] => Table[rpID];

        internal AirBase()
        {
            ApiService.Subscribe("api_req_air_corps/set_action", r =>
            {
                var rGroups = r.Parameters["api_base_id"].Split(',').Select(rpID => Table[int.Parse(rpID)]).ToArray();
                var rOptions = r.Parameters["api_action_kind"].Split(',').Select(rpOption => (AirForceGroupOption)int.Parse(rpOption)).ToArray();

                for (int i = 0; i < rGroups.Length; i++)
                    rGroups[i].Option = rOptions[i];
            });

            ApiService.Subscribe("api_req_air_corps/change_name", r =>
            {
                var rGroup = Table[int.Parse(r.Parameters["api_base_id"])];
                rGroup.Name = r.Parameters["api_name"];
            });

            ApiService.Subscribe("api_req_air_corps/set_plane", r =>
            {
                var rGroup = Table[int.Parse(r.Parameters["api_base_id"])];

                var rData = r.GetData<RawAirForceGroupOrganization>();
                foreach (var rSquadron in rData.Squadrons)
                    rGroup.Squadrons[rSquadron.ID].Update(rSquadron);

                rGroup.CombatRadius = rData.CombatRadius;
                rGroup.UpdateFighterPower();
            });

            ApiService.Subscribe("api_req_air_corps/supply", r =>
            {
                var rGroup = Table[int.Parse(r.Parameters["api_base_id"])];

                var rData = r.GetData<RawAirForceSquadronResupplyResult>();
                foreach (var rSquadron in rData.Squadrons)
                    rGroup.Squadrons[rSquadron.ID].Update(rSquadron);

                rGroup.UpdateFighterPower();
            });
        }

        internal void UpdateGroups(RawAirForceGroup[] rpGroups)
        {
            if (Table == null)
                Table = new IDTable<AirForceGroup>();

            if (Table.UpdateRawData(rpGroups, r => new AirForceGroup(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
                OnPropertyChanged(nameof(Table));
        }
    }
}
