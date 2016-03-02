using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models;
using System;
using System.IO;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public class ExpeditionService
    {
        public const string DataFilename = @"Data\expeditions.json";

        public static ExpeditionService Instance { get; } = new ExpeditionService();

        IDisposable r_ConnectionSubscription;

        IDTable<ExpeditionInfo2> r_Infos;

        ExpeditionService() { }

        public void Initialize()
        {
            r_ConnectionSubscription = SessionService.Instance.Subscribe("api_get_member/basic", _ =>
            {
                var rDataFile = new FileInfo(DataFilename);
                if (!rDataFile.Exists)
                    r_Infos = new IDTable<ExpeditionInfo2>();
                else
                    using (var rReader = new JsonTextReader(rDataFile.OpenText()))
                    {
                        var rData = JArray.Load(rReader);

                        r_Infos = new IDTable<ExpeditionInfo2>(rData.ToObject<ExpeditionInfo2[]>().ToDictionary(r => r.ID));
                    }

                r_ConnectionSubscription?.Dispose();
                r_ConnectionSubscription = null;
            });
        }

        public bool ContainsInfo(int rpID) => r_Infos.ContainsKey(rpID);

        public ExpeditionInfo2 GetInfo(int rpID)
        {
            ExpeditionInfo2 rInfo;
            r_Infos.TryGetValue(rpID, out rInfo);
            return rInfo;
        }
    }
}
