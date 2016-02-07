using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services
{
    public class ShipLockingService
    {
        public const string DataFilename = @"Data\ship_locking.json";

        public static ShipLockingService Instance { get; } = new ShipLockingService();

        IDTable<ShipLocking> r_ShipLocking = new IDTable<ShipLocking>();
        public IList<ShipLocking> ShipLocking { get; private set; }

        ShipLockingService() { }

        public void Initialize()
        {
            var rDataFile = new FileInfo(DataFilename);
            if (!rDataFile.Exists)
                r_ShipLocking = new IDTable<ShipLocking>();
            else
                using (var rReader = new JsonTextReader(rDataFile.OpenText()))
                    r_ShipLocking = new IDTable<ShipLocking>(JArray.Load(rReader).Select(r => r.ToObject<ShipLocking>()));

            ShipLocking = r_ShipLocking.Values.ToList().AsReadOnly();
        }

        public ShipLocking GetLocking(int rpID)
        {
            ShipLocking rResult;
            r_ShipLocking.TryGetValue(rpID, out rResult);
            return rResult;
        }
    }
}
