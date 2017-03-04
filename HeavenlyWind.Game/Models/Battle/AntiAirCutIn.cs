using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle
{
    public class AntiAirCutIn : ModelBase
    {
        static IDictionary<int, int> r_ShotdownCount;

        public int TypeID { get; }

        public int? ShotdownCount { get; }

        public EquipmentInfo[] UsedEquipment { get; }

        static AntiAirCutIn()
        {
            DataStore.Updated += rpName =>
            {
                if (rpName == "anti_air_cut_in")
                    Reload();
            };

            Reload();
        }
        internal AntiAirCutIn(RawAntiAirCutIn rpData)
        {
            TypeID = rpData.TypeID;

            UsedEquipment = rpData.EquipmentIDs.Select(r => KanColleGame.Current.MasterInfo.Equipment[r]).ToArray();

            int rShotdownCount;
            if (r_ShotdownCount.TryGetValue(TypeID, out rShotdownCount))
                ShotdownCount = rShotdownCount;
        }

        static void Reload()
        {
            byte[] rContent;
            if (!DataStore.TryGet("anti_air_cut_in", out rContent))
            {
                r_ShotdownCount = new ListDictionary<int, int>();
                return;
            }

            var rReader = new JsonTextReader(new StreamReader(new MemoryStream(rContent)));
            var rData = JArray.Load(rReader);

            r_ShotdownCount = rData.ToSortedList(r => (int)r["id"], r => (int)r["shotdown"]);
        }
    }
}
