using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Events;
using Sakuno.KanColle.Amatsukaze.Game.Json;
using Sakuno.KanColle.Amatsukaze.Game.Json.MasterData;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Messaging;
using Sakuno.KanColle.Amatsukaze.Services;

namespace Sakuno.KanColle.Amatsukaze.Game
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
            BuildingDockUpdated = requireInfo.Select(x => x.SelectResponse(r => r.api_kdock))
                .CombineWith<ITimedMessage<IReadOnlyCollection<BuildingDockJson>>>(RegisterRaw<BuildingDockJson[]>("api_get_member/kdock"));
            UseItemUpdated = requireInfo.Select(x => x.SelectResponse(r => r.api_useitem))
                .CombineWith<ITimedMessage<IReadOnlyCollection<UseItemCountJson>>>(RegisterRaw<UseItemCountJson[]>("api_get_member/useitem"));
            FreeEquipmentUpdated = requireInfo.Select(x => x.SelectResponse(r => r.api_unsetslot))
                .CombineWith(RegisterRaw<Dictionary<string, int[]>>("api_get_member/unsetslot"));

            var homeport = RegisterRaw<HomeportJson>("api_port/port");
            AdmiralUpdated = homeport.Select(x => x.SelectResponse(r => r.api_basic))
                .CombineWith<ITimedMessage<IRawAdmiral>>(RegisterRaw<AdmiralRecordJson>("api_get_member/record"));
            MaterialsUpdated = homeport.Select(x => x.SelectResponse(r => new MaterialUpdates(r.api_material)))
                .CombineWith(RegisterRaw<MaterialJson[]>("api_get_member/material").Select(x => x.SelectResponse(r => new MaterialUpdates(r))));
            RepairingDockUpdated = homeport.Select(x => x.SelectResponse(r => r.api_ndock))
                .CombineWith<ITimedMessage<IReadOnlyCollection<RepairingDockJson>>>(RegisterRaw<RepairingDockJson[]>("api_get_member/ndock"));
            HomeportUpdated = homeport.Select(x => x.SelectResponse(ParseHomeport));
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
