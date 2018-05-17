using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models;
using System.IO;
using System.Linq;
using System.Threading;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    class ExpeditionService
    {
        public static ExpeditionService Instance { get; } = new ExpeditionService();

        ManualResetEventSlim r_InitializationLock = new ManualResetEventSlim(false);

        IDTable<ExpeditionInfo2> r_Infos;

        ExpeditionService() { }

        public void Initialize()
        {
            ApiService.SubscribeOnce("api_get_member/require_info", delegate
            {
                try
                {
                    byte[] rContent;
                    if (!DataStore.TryGet("expedition", out rContent))
                        r_Infos = new IDTable<ExpeditionInfo2>();
                    else
                    {
                        var rReader = new JsonTextReader(new StreamReader(new MemoryStream(rContent)));
                        var rData = JArray.Load(rReader);

                        r_Infos = rData.Select(r => r.ToObject<ExpeditionInfo2>()).ToIDTable();
                    }
                }
                finally
                {
                    r_InitializationLock.Set();
                    r_InitializationLock.Dispose();
                    r_InitializationLock = null;
                }
            });
        }

        public void WaitForInitialization() => r_InitializationLock?.Wait();

        public bool ContainsInfo(int rpID) => r_Infos.ContainsKey(rpID);

        public ExpeditionInfo2 GetInfo(int rpID) => r_Infos.GetValueOrDefault(rpID);
    }
}
