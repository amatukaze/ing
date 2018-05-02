using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Json.MasterData;
using Sakuno.ING.Game.Json.Shipyard;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Messaging;
using Sakuno.ING.Services;

namespace Sakuno.ING.Game
{
    public sealed partial class GameListener
    {
        private ITextStreamProvider provider;
        private JsonSerializer jSerializer = new JsonSerializer
        {
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        public GameListener(ITextStreamProvider provider)
        {
            this.provider = provider;
            jSerializer.Error += JsonError;

            MasterDataUpdated = RegisterRaw<MasterDataJson>("api_start2")
                .Select(x => x.SelectResponse(ParseMasterData));

            var requireInfo = RegisterRaw<GameStartupInfoJson>("api_get_member/require_info");
            AllEquipmentUpdated = requireInfo.Select(x => x.SelectResponse(r => r.api_slot_item))
                .CombineWith<ITimedMessage<IReadOnlyCollection<EquipmentJson>>>(RegisterRaw<EquipmentJson[]>("api_get_member/slot_item"));
            UseItemUpdated = requireInfo.Select(x => x.SelectResponse(r => r.api_useitem))
                .CombineWith<ITimedMessage<IReadOnlyCollection<UseItemCountJson>>>(RegisterRaw<UseItemCountJson[]>("api_get_member/useitem"));
            FreeEquipmentUpdated = requireInfo.Select(x => x.SelectResponse(r => r.api_unsetslot))
                .CombineWith(RegisterRaw<Dictionary<string, int[]>>("api_get_member/unsetslot"));

            var homeport = RegisterRaw<HomeportJson>("api_port/port");
            AdmiralUpdated = homeport.Select(x => x.SelectResponse(r => r.api_basic))
                .CombineWith<ITimedMessage<IRawAdmiral>>(RegisterRaw<AdmiralRecordJson>("api_get_member/record"));
            RepairingDockUpdated = homeport.Select(x => x.SelectResponse(r => r.api_ndock))
                .CombineWith<ITimedMessage<IReadOnlyCollection<RepairingDockJson>>>(RegisterRaw<RepairingDockJson[]>("api_get_member/ndock"));
            HomeportUpdated = homeport;

            RepairStarted = RegisterRaw("api_req_nyukyo/start")
                .Select(x => x.SelectRequest(ParseRepairStart));
            InstantRepaired = RegisterRaw("api_req_nyukyo/speedchange")
                .Select(x => x.SelectRequest(ParseInstantRepair));
            ShipCreated = RegisterRaw("api_req_kousyou/createship")
                .Select(x => x.SelectRequest(ParseShipCreation));
            InstantBuilt = RegisterRaw("api_req_kousyou/createship_speedchange")
                .Select(x => x.SelectRequest(ParseInstantBuilt));

            var getShip = RegisterRaw<ShipBuildCompletionJson>("api_req_kousyou/getship");
            BuildingDockUpdated = requireInfo.Select(x => x.SelectResponse(r => r.api_kdock))
                .CombineWith<ITimedMessage<IReadOnlyCollection<BuildingDockJson>>>
                    (RegisterRaw<BuildingDockJson[]>("api_get_member/kdock"),
                    getShip.Select(x => x.SelectResponse(r => r.api_kdock)));
            ShipBuildCompleted = getShip;

            var createItem = RegisterRaw<EquipmentCreationJson>("api_req_kousyou/createitem");
            EquipmentCreated = createItem.Select(x => x.SelectRequestAndResponse(ParseEquipmentCreation));

            var destroyShip = RegisterRaw<ShipDismantleJson>("api_req_kousyou/destroyship");
            ShipDismantled = destroyShip.Select(x => x.SelectRequest(ParseShipDismantling));

            var destroyItem = RegisterRaw<EquipmentDismantleJson>("api_req_kousyou/destroyitem2");
            MaterialsUpdated = homeport.Select(x => x.SelectResponse(r => r.api_material))
                .CombineWith<ITimedMessage<IMaterialsUpdate>>
                    (RegisterRaw<MaterialJsonArray>("api_get_member/material"),
                    createItem, destroyShip, destroyItem);
            EquipmentDismantled = destroyItem.Select(x => x.SelectRequest(ParseEquipmentDimantling));

            EquipmentImproved = RegisterRaw<EquipmentImproveJson>("api_req_kousyou/remodel_slot")
                .Select(x => x.SelectRequestAndResponse(ParseEquipmentImprove));
        }

        private void JsonError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            //Debug usage
        }

        private T Convert<T>(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return jSerializer.Deserialize<T>(new JsonTextReader(new StreamReader(stream)));
        }

        public IProducer<ParsedMessage<T>> RegisterRaw<T>(string api) => provider
            .Where(arg => arg.Key.Equals(api, StringComparison.Ordinal))
            .Select(arg => new ParsedMessage<T>
            (
                arg.Key,
                arg.TimeStamp,
                HttpUtility.ParseQueryString(arg.Request),
                Convert<SvData<T>>(arg.Stream)
            ))
            .Where(m => m.IsSuccess);

        public IProducer<ParsedMessage> RegisterRaw(string api) => provider
            .Where(arg => arg.Key.Equals(api, StringComparison.Ordinal))
            .Select(arg => new ParsedMessage
            (
                arg.Key,
                arg.TimeStamp,
                HttpUtility.ParseQueryString(arg.Request),
                Convert<SvData>(arg.Stream)
            ))
            .Where(m => m.IsSuccess);

        public IProducer<SvData> RegisterFail() => provider
            .Select(arg => Convert<SvData>(arg.Stream))
            .Where(v => v.api_result != 1);

        public IProducer<ParsedMessage<JToken>> RegisterAny() => provider
            .Select(arg => new ParsedMessage<JToken>
            (
                arg.Key,
                arg.TimeStamp,
                HttpUtility.ParseQueryString(arg.Request),
                Convert<SvData<JToken>>(arg.Stream)
            ));
    }
}
