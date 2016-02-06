using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.IO;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public class ShipLockingService
    {
        public const string DataFilename = @"Data\ship_locking.json";

        public static ShipLockingService Instance { get; } = new ShipLockingService();

        IDisposable r_GameInitializedSubscription;

        IDTable<ShipLocking> r_ShipLocking = new IDTable<ShipLocking>();

        ShipLockingService() { }

        public void Initialize()
        {
            r_GameInitializedSubscription = SessionService.Instance.Subscribe("api_get_member/basic", _ =>
            {
                var rDataFile = new FileInfo(DataFilename);
                if (!rDataFile.Exists)
                    r_ShipLocking = new IDTable<ShipLocking>();
                else
                    using (var rReader = new JsonTextReader(rDataFile.OpenText()))
                        r_ShipLocking = new IDTable<ShipLocking>(JArray.Load(rReader).Select(r => r.ToObject<ShipLocking>()));

                r_GameInitializedSubscription?.Dispose();
                r_GameInitializedSubscription = null;
            });
        }

        public ShipLocking GetLocking(int rpID)
        {
            ShipLocking rResult;
            r_ShipLocking.TryGetValue(rpID, out rResult);
            return rResult;
        }
    }
}
