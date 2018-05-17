using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Collections.Generic;
using System.IO;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public class ShipLockingService
    {
        public static ShipLockingService Instance { get; } = new ShipLockingService();

        public IList<ShipLocking> ShipLocking { get; private set; }

        ShipLockingService() { }

        internal void Initialize()
        {
            byte[] rContent;
            if (!DataStore.TryGet("ship_locking", out rContent))
                return;

            var rReader = new JsonTextReader(new StreamReader(new MemoryStream(rContent)));

            ShipLocking = JArray.Load(rReader).ToObject<ShipLocking[]>();
        }

        public ShipLocking GetLocking(int rpID)
        {
            if (ShipLocking == null || rpID <= 0 || rpID > ShipLocking.Count)
                return null;

            return ShipLocking[rpID - 1];
        }
    }
}
