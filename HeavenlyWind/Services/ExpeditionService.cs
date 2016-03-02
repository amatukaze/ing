using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models;
using System.IO;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public class ExpeditionService
    {
        public const string DataFilename = @"Data\expeditions.json";

        public static ExpeditionService Instance { get; } = new ExpeditionService();

        IDTable<ExpeditionInfo2> r_Infos;

        ExpeditionService() { }

        public void Initialize()
        {
            SessionService.Instance.SubscribeOnce("api_get_member/basic", delegate
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
