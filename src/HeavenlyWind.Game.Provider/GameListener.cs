using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Json;
using Sakuno.KanColle.Amatsukaze.Game.Json.MasterData;
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
                .Select(ParseMasterData);

            var requireInfo = RegisterRaw<GameStartupInfoJson>("api_get_member/require_info");
            AllEquipmentUpdated = requireInfo.Select(x => x.Response.api_slot_item)
                .CombineWith(RegisterRaw<EquipmentJson[]>("api_get_member/slot_item").Select(x => x.Response));
            BuildingDockUpdated = requireInfo.Select(x => x.Response.api_kdock)
                .CombineWith(RegisterRaw<BuildingDockJson[]>("api_get_member/kdock").Select(x => x.Response));
            UseItemUpdated = requireInfo.Select(x => x.Response.api_useitem)
                .CombineWith(RegisterRaw<UseItemCountJson[]>("api_get_member/useitem").Select(x => x.Response));
            FreeEquipmentUpdated = requireInfo.Select(x => x.Response.api_unsetslot)
                .CombineWith(RegisterRaw<Dictionary<string, int[]>>("api_get_member/unsetslot").Select(x => x.Response));
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
