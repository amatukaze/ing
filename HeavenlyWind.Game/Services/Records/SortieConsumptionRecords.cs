using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Events;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    class SortieConsumptionRecords : RecordsGroup
    {
        public override string GroupName => "sortie_consumption";
        public override int Version => 2;

        Port Port = KanColleGame.Current.Port;

        enum ShipParticipantType { Sortie, SupportFire, NormalExpedition, Practice }
        enum ConsumptionType { Supply, Repair, Remodel }

        ListDictionary<int, SupplySnapshot> r_SupplySnapshots = new ListDictionary<int, SupplySnapshot>();

        bool r_AnchorageRepairSnapshotsInitialized;
        long r_AnchorageRepairStartTime;
        ListDictionary<int, AnchorageRepairSnapshot> r_AnchorageRepairSnapshots = new ListDictionary<int, AnchorageRepairSnapshot>();

        public SortieConsumptionRecords(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(ApiService.Subscribe("api_req_map/start", StartSortie));

            DisposableObjects.Add(ApiService.Subscribe("api_req_hokyu/charge", BeforeSupply, AfterSupply));

            DisposableObjects.Add(ApiService.Subscribe("api_req_nyukyo/start", Repair));
            DisposableObjects.Add(ApiService.SubscribeOnlyOnBeforeProcessStarted("api_req_nyukyo/speedchange", UseBucket));

            DisposableObjects.Add(ApiService.Subscribe("api_req_hensei/change", ResetAnchorageRepairSnapshots));
            DisposableObjects.Add(ApiService.Subscribe("api_port/port", ProcessAnchorageRepair));

            DisposableObjects.Add(ApiService.Subscribe("api_req_kaisou/remodeling", Remodel));

            DisposableObjects.Add(ApiService.Subscribe(new[] { "api_req_map/start", "api_req_map/next" }, Exploration));

            DisposableObjects.Add(ApiService.Subscribe("api_req_practice/battle", StartPractice));

            DisposableObjects.Add(ApiService.Subscribe("api_req_mission/result", ExpeditionResult));
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText =
                    "CREATE TABLE IF NOT EXISTS sortie_consumption(id INTEGER PRIMARY KEY NOT NULL); " +

                    "CREATE TABLE IF NOT EXISTS sortie_participant_ship(" +
                        "id INTEGER NOT NULL REFERENCES sortie_consumption(id) ON DELETE CASCADE ON UPDATE CASCADE, " +
                        "ship_id INTEGER NOT NULL, " +
                        "ship INTEGER NOT NULL, " +
                        "type INTEGER NOT NULL, " +
                        "PRIMARY KEY(id, ship_id)) WITHOUT ROWID; " +

                    "CREATE TABLE IF NOT EXISTS sortie_consumption_detail(" +
                        "id INTEGER NOT NULL REFERENCES sortie_consumption(id) ON DELETE CASCADE ON UPDATE CASCADE, " +
                        "type INTEGER NOT NULL, " +
                        "fuel INTEGER, " +
                        "bullet INTEGER, " +
                        "steel INTEGER, " +
                        "bauxite INTEGER, " +
                        "bucket INTEGER, " +
                        "PRIMARY KEY(id, type)) WITHOUT ROWID; " +

                    "CREATE TABLE IF NOT EXISTS anchorage_repair(" +
                        "ship INTEGER PRIMARY KEY NOT NULL, " +
                        "hp INTEGER NOT NULL, " +
                        "repair_time REAL NOT NULL, " +
                        "fuel_consumption INTEGER NOT NULL, " +
                        "steel_consumption INTEGER NOT NULL); " +

                    "CREATE TABLE IF NOT EXISTS sortie_reward(" +
                        "id INTEGER PRIMARY KEY NOT NULL REFERENCES sortie_consumption(id) ON DELETE CASCADE ON UPDATE CASCADE, " +
                        "fuel INTEGER, " +
                        "bullet INTEGER, " +
                        "steel INTEGER, " +
                        "bauxite INTEGER, " +
                        "bucket INTEGER); ";

                rCommand.ExecuteNonQuery();
            }
        }

        protected override void Load()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "SELECT ifnull(value, 0) FROM common WHERE key = 'anchorage_repair_start_time';";

                r_AnchorageRepairStartTime = Convert.ToInt64(rCommand.ExecuteScalar());

                rCommand.CommandText = "SELECT * FROM anchorage_repair;";

                using (var rReader = rCommand.ExecuteReader())
                    while (rReader.Read())
                    {
                        var rShipID = Convert.ToInt32(rReader["ship"]);

                        var rHP = Convert.ToInt32(rReader["hp"]);
                        var rRepairTime = Convert.ToDouble(rReader["repair_time"]);
                        var rFuelConsumption = Convert.ToInt32(rReader["fuel_consumption"]);
                        var rSteelConsumption = Convert.ToInt32(rReader["steel_consumption"]);

                        r_AnchorageRepairSnapshots.Add(rShipID, new AnchorageRepairSnapshot(rShipID, rHP, rRepairTime, rFuelConsumption, rSteelConsumption));
                    }
            }
        }

        void StartSortie(ApiInfo rpData)
        {
            var rSortie = SortieInfo.Current;

            var rSupportFleets = KanColleGame.Current.Port.Fleets.Table.Values
                .Where(r =>
                {
                    var rExpedition = r.ExpeditionStatus.Expedition;

                    return rExpedition != null && !rExpedition.CanReturn && rExpedition.MapArea.ID == SortieInfo.Current.Map.MasterInfo.AreaID;
                }).ToArray();

            using (var rTransaction = Connection.BeginTransaction())
            using (var rCommand = Connection.CreateCommand())
            {
                IEnumerable<IParticipant> rParticipants = rSortie.MainShips;
                if (rSortie.EscortShips != null)
                    rParticipants = rParticipants.Concat(rSortie.EscortShips);

                var rBuilder = new StringBuilder(128);
                rBuilder.Append("INSERT INTO sortie_consumption(id) VALUES (@id); ");
                rBuilder.Append("INSERT INTO sortie_participant_ship(id, ship_id, ship, type) VALUES");

                var rCount = 0;
                foreach (FriendShip rParticipant in rParticipants)
                {
                    if (rCount++ > 0)
                        rBuilder.Append(", ");

                    rBuilder.Append($"(@id, {rParticipant.Ship.ID}, {rParticipant.Info.ID}, 0)");

                    rCount++;
                }

                if (rSupportFleets.Length > 0)
                    foreach (var rShip in rSupportFleets.SelectMany(r => r.Ships))
                        rBuilder.Append($", (@id, {rShip.ID}, {rShip.Info.ID}, 1)");

                rBuilder.Append(';');

                rCommand.CommandText = rBuilder.ToString();
                rCommand.Parameters.AddWithValue("@id", rSortie.ID);

                rCommand.ExecuteNonQuery();

                rTransaction.Commit();
            }
        }

        void BeforeSupply(ApiInfo rpInfo)
        {
            var rData = rpInfo.GetData<RawSupplyResult>();

            r_SupplySnapshots.Clear();

            foreach (var rShipSupplyResult in rData.Ships)
            {
                var rShip = Port.Ships[rShipSupplyResult.ID];
                var rPlaneCount = rShip.Slots.Take(rShip.Info.SlotCount).Sum(r => r.PlaneCount);

                r_SupplySnapshots.Add(rShip.ID, new SupplySnapshot(rShip.IsMarried, rShip.Fuel.Current, rShip.Bullet.Current, rShip.Info.SlotCount, rPlaneCount));
            }
        }
        void AfterSupply(ApiInfo rpInfo)
        {
            var rData = rpInfo.GetData<RawSupplyResult>();

            using (var rTransaction = Connection.BeginTransaction())
            using (var rCommand = Connection.CreateCommand())
            {
                var rBuilder = new StringBuilder(256);
                rBuilder.Append("INSERT OR IGNORE INTO sortie_consumption_detail(id, type) VALUES");

                var rCount = 0;
                foreach (var rShipSupplyResult in rData.Ships)
                {
                    if (rCount > 0)
                        rBuilder.Append(", ");

                    rBuilder.Append($"((SELECT max(id) FROM sortie_participant_ship WHERE ship_id = {rShipSupplyResult.ID}), 0)");

                    rCount++;
                }
                rBuilder.Append("; ");

                foreach (var rShipSupplyResult in rData.Ships)
                {
                    var rSnapshot = r_SupplySnapshots[rShipSupplyResult.ID];

                    var rFuelDiff = rShipSupplyResult.Fuel - rSnapshot.Fuel;
                    var rBulletDiff = rShipSupplyResult.Bullet - rSnapshot.Bullet;

                    if (rSnapshot.IsMarried)
                    {
                        rFuelDiff = (int)(rFuelDiff * .85);
                        rBulletDiff = (int)(rBulletDiff * .85);
                    }

                    var rBauxiteDiff = (rShipSupplyResult.Planes.Take(rSnapshot.SlotCount).Sum() - rSnapshot.Plane) * 5;

                    rBuilder.Append($"UPDATE sortie_consumption_detail SET fuel = coalesce(fuel, 0) + {rFuelDiff}, bullet = coalesce(bullet, 0) + {rBulletDiff}, bauxite = coalesce(bauxite, 0) + {rBauxiteDiff} WHERE id = (SELECT max(id) FROM sortie_participant_ship WHERE ship_id = {rShipSupplyResult.ID}) AND type = 0; ");
                }

                rCommand.CommandText = rBuilder.ToString();
                rCommand.ExecuteNonQuery();

                rTransaction.Commit();
            }

            r_SupplySnapshots.Clear();
        }

        void Repair(ApiInfo rpInfo)
        {
            var rShip = Port.Ships[int.Parse(rpInfo.Parameters["api_ship_id"])];
            var rUseBucket = rpInfo.Parameters["api_highspeed"] == "1";

            using (var rTransaction = Connection.BeginTransaction())
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText =
                    "INSERT OR IGNORE INTO sortie_consumption_detail(id, type) VALUES((SELECT max(id) FROM sortie_participant_ship WHERE ship_id = @ship_id AND type = 0), 1);" +
                    "UPDATE sortie_consumption_detail SET fuel = coalesce(fuel, 0) + @fuel, steel = coalesce(steel, 0) + @steel, bucket = coalesce(bucket, 0) + @bucket WHERE id = (SELECT max(id) FROM sortie_participant_ship WHERE ship_id = @ship_id AND type = 0) AND type = 1;";
                rCommand.Parameters.AddWithValue("@ship_id", rShip.ID);
                rCommand.Parameters.AddWithValue("@fuel", rShip.RepairFuelConsumption);
                rCommand.Parameters.AddWithValue("@steel", rShip.RepairSteelConsumption);
                rCommand.Parameters.AddWithValue("@bucket", rUseBucket ? 1 : 0);

                rCommand.ExecuteNonQuery();

                rTransaction.Commit();
            }
        }
        void UseBucket(ApiInfo rpInfo)
        {
            var rDock = Port.RepairDocks[int.Parse(rpInfo.Parameters["api_ndock_id"])];

            using (var rTransaction = Connection.BeginTransaction())
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText =
                    "INSERT OR IGNORE INTO sortie_consumption_detail(id, type) VALUES((SELECT max(id) FROM sortie_participant_ship WHERE ship_id = @ship_id AND type = 0), 1);" +
                    "UPDATE sortie_consumption_detail SET bucket = coalesce(bucket, 0) + 1 WHERE id = (SELECT max(id) FROM sortie_participant_ship WHERE ship_id = @ship_id AND type = 0) AND type = 1;";
                rCommand.Parameters.AddWithValue("@ship_id", rDock.Ship.ID);

                rCommand.ExecuteNonQuery();

                rTransaction.Commit();
            }
        }

        void ResetAnchorageRepairSnapshots(ApiInfo rpInfo)
        {
            using (var rTransaction = Connection.BeginTransaction())
            using (var rCommand = Connection.CreateCommand())
            {
                var rBuilder = new StringBuilder(128);
                rBuilder.Append("DELETE FROM anchorage_repair; ");
                rBuilder.Append("DELETE FROM common WHERE key = 'anchorage_repair_start_time'; ");

                r_AnchorageRepairSnapshots.Clear();

                var rShips = Port.Fleets.Table.Values.SelectMany(r => r.AnchorageRepair.RepairingShips).ToArray();
                if (rShips.Length == 0)
                    r_AnchorageRepairStartTime = 0;
                else
                {
                    r_AnchorageRepairStartTime = rpInfo.Timestamp;

                    rBuilder.Append("INSERT INTO anchorage_repair(ship, hp, repair_time, fuel_consumption, steel_consumption) VALUES");

                    var rCount = 0;
                    foreach (var rShip in rShips)
                    {
                        if (rCount++ > 0)
                            rBuilder.Append(", ");

                        r_AnchorageRepairSnapshots.Add(rShip.ID, new AnchorageRepairSnapshot(rShip, rShip.HP.Current, rShip.RepairTime.Value.TotalMinutes, rShip.RepairFuelConsumption, rShip.RepairSteelConsumption));

                        rBuilder.Append($"({rShip.ID}, {rShip.HP.Current}, {rShip.RepairTime.Value.TotalMinutes}, {rShip.RepairFuelConsumption}, {rShip.RepairSteelConsumption})");

                        rCount++;
                    }
                    rBuilder.Append(';');

                    rBuilder.Append($"INSERT OR REPLACE INTO common(key, value) VALUES('anchorage_repair_start_time', {r_AnchorageRepairStartTime});");
                }

                rCommand.CommandText = rBuilder.ToString();

                rCommand.ExecuteNonQuery();

                rTransaction.Commit();
            }
        }
        void ProcessAnchorageRepair(ApiInfo rpInfo)
        {
            InitializeAnchorageRepairSnapshots(rpInfo);

            var rSnapshots = r_AnchorageRepairSnapshots.Values.Where(r => r.Ship.HP.Current > r.HP).ToArray();
            if (rSnapshots.Length == 0)
                return;

            using (var rTransaction = Connection.BeginTransaction())
            using (var rCommand = Connection.CreateCommand())
            {
                var rBuilder = new StringBuilder(256);
                rBuilder.Append("INSERT OR IGNORE INTO sortie_consumption_detail(id, type) VALUES");

                var rCount = 0;
                foreach (var rSnapshot in rSnapshots)
                {
                    if (rCount > 0)
                        rBuilder.Append(", ");

                    rBuilder.Append($"((SELECT max(id) FROM sortie_participant_ship WHERE ship_id = {rSnapshot.ShipID}), 4)");

                    rCount++;
                }
                rBuilder.Append("; ");

                var rRemovedIDs = new List<int>();

                var rTimeDiff = Math.Ceiling((rpInfo.Timestamp - r_AnchorageRepairStartTime) / 60.0 / 20.0);

                foreach (var rSnapshot in rSnapshots)
                {
                    var rRate = Math.Min(1.0, rTimeDiff / Math.Ceiling(rSnapshot.RepairTime / 20.0));

                    var rFuelConsumption = Math.Ceiling(rSnapshot.FuelConsumption * rRate);
                    var rSteelConsumption = Math.Ceiling(rSnapshot.SteelConsumption * rRate);

                    rBuilder.Append($"UPDATE sortie_consumption_detail SET fuel = ifnull(fuel, 0) + {rFuelConsumption}, steel = ifnull(steel, 0) + {rSteelConsumption} WHERE id = (SELECT max(id) FROM sortie_participant_ship WHERE ship_id = {rSnapshot.ShipID}) AND type = 4; ");

                    var rShip = rSnapshot.Ship;
                    if (rShip.HP.Current < rShip.HP.Maximum)
                        r_AnchorageRepairSnapshots[rSnapshot.ShipID] = new AnchorageRepairSnapshot(rShip, rShip.HP.Current, rShip.RepairTime.Value.TotalMinutes, rShip.RepairFuelConsumption, rShip.RepairSteelConsumption);
                    else
                    {
                        rRemovedIDs.Add(rSnapshot.ShipID);
                        r_AnchorageRepairSnapshots.Remove(rSnapshot.ShipID);
                    }
                }

                if (rRemovedIDs.Count > 0)
                    rBuilder.Append("DELETE FROM anchorage_repair WHERE ship IN (" + string.Join(", ", rRemovedIDs) + "); ");

                if (r_AnchorageRepairSnapshots.Count == 0)
                {
                    r_AnchorageRepairStartTime = 0;
                    rBuilder.Append("DELETE FROM common WHERE key = 'anchorage_repair_start_time';");
                }
                else
                {
                    r_AnchorageRepairStartTime = rpInfo.Timestamp;
                    rBuilder.Append($"INSERT OR REPLACE INTO common(key, value) VALUES('anchorage_repair_start_time', {r_AnchorageRepairStartTime});");

                    rBuilder.Append("INSERT OR REPLACE INTO anchorage_repair(ship, hp, repair_time, fuel_consumption, steel_consumption) VALUES");

                    rCount = 0;
                    foreach (var rSnapshot in r_AnchorageRepairSnapshots.Values)
                    {
                        if (rCount++ > 0)
                            rBuilder.Append(", ");

                        rBuilder.Append($"({rSnapshot.ShipID}, {rSnapshot.HP}, {rSnapshot.RepairTime}, {rSnapshot.FuelConsumption}, {rSnapshot.SteelConsumption})");

                        rCount++;
                    }
                    rBuilder.Append(';');
                }

                rCommand.CommandText = rBuilder.ToString();
                rCommand.ExecuteNonQuery();

                rTransaction.Commit();
            }
        }
        void InitializeAnchorageRepairSnapshots(ApiInfo rpInfo)
        {
            if (r_AnchorageRepairSnapshotsInitialized)
                return;

            var rRepairingShips = Port.Fleets.Table.Values.SelectMany(r => r.AnchorageRepair.RepairingShips);
            var rAbsentShipIDs = r_AnchorageRepairSnapshots.Keys.Except(rRepairingShips.Select(r => r.ID)).Where(r =>
            {
                Ship rShip;
                if (!Port.Ships.TryGetValue(r, out rShip))
                    return true;

                return rShip.HP.Current < rShip.HP.Maximum;
            }).ToArray();

            using (var rTranscation = Connection.BeginTransaction())
            using (var rCommand = Connection.CreateCommand())
            {
                if (rAbsentShipIDs.Length > 0)
                {
                    rCommand.CommandText = "DELETE FROM anchorage_repair WHERE ship IN (" + string.Join(", ", rAbsentShipIDs) + ");";
                    rCommand.ExecuteNonQuery();

                    foreach (var rID in rAbsentShipIDs)
                        r_AnchorageRepairSnapshots.Remove(rID);
                }

                foreach (var rShip in rRepairingShips.Where(r => !r_AnchorageRepairSnapshots.ContainsKey(r.ID)))
                    r_AnchorageRepairSnapshots[rShip.ID] = new AnchorageRepairSnapshot(rShip, rShip.HP.Current, rShip.RepairTime.Value.TotalMinutes, rShip.RepairFuelConsumption, rShip.RepairSteelConsumption);

                if (r_AnchorageRepairSnapshots.Count == 0)
                {
                    r_AnchorageRepairStartTime = 0;

                    rCommand.CommandText = "DELETE FROM common WHERE key = 'anchorage_repair_start_time';";
                    rCommand.ExecuteNonQuery();
                }

                rTranscation.Commit();
            }

            r_AnchorageRepairSnapshotsInitialized = true;
        }

        void Remodel(ApiInfo rpInfo)
        {
            var rShip = Port.Ships[int.Parse(rpInfo.Parameters["api_id"])];

            using (var rTransaction = Connection.BeginTransaction())
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText =
                    "INSERT OR IGNORE INTO sortie_consumption_detail(id, type) VALUES((SELECT max(id) FROM sortie_participant_ship WHERE ship_id = @ship_id), 2);" +
                    "UPDATE sortie_consumption_detail SET bullet = coalesce(bullet, 0) + @bullet, steel = coalesce(steel, 0) + @steel WHERE id = (SELECT max(id) FROM sortie_participant_ship WHERE ship_id = @ship_id) AND type = 2;";
                rCommand.Parameters.AddWithValue("@ship_id", rShip.ID);
                rCommand.Parameters.AddWithValue("@bullet", rShip.Info.RemodelingFuelConsumption);
                rCommand.Parameters.AddWithValue("@steel", rShip.Info.RemodelingBulletConsumption);

                rCommand.ExecuteNonQuery();

                rTransaction.Commit();
            }
        }

        void Exploration(ApiInfo rpInfo)
        {
            var rNode = SortieInfo.Current.Node;
            var rEvent = rNode.Event as RewardEventBase;
            if (rEvent == null)
                return;

            using (var rTransaction = Connection.BeginTransaction())
            using (var rCommand = Connection.CreateCommand())
            {
                IList<string> rSetters = null;

                switch (rNode.EventType)
                {
                    case SortieEventType.Reward:
                    case SortieEventType.AviationReconnaissance:
                        var rRewards = ((RewardEvent)rEvent).Rewards;
                        if (rRewards == null || rRewards.Count == 0)
                            return;

                        foreach (var rReward in rRewards)
                        {
                            var rSetter = GetRewardSetter(rReward.ID, rReward.Quantity);
                            if (rSetter == null)
                                continue;

                            if (rSetters == null)
                                rSetters = new List<string>();

                            rSetters.Add(rSetter);
                        }
                        break;

                    case SortieEventType.EscortSuccess:
                        rSetters = new[] { GetRewardSetter(rEvent.ID, rEvent.Quantity) };
                        break;

                    default:
                        return;
                }

                if (rSetters == null || rSetters.Count == 0)
                    return;

                rCommand.CommandText =
                    "INSERT OR IGNORE INTO sortie_reward(id) VALUES(@id);" +
                    "UPDATE sortie_reward SET " +
                        rSetters.Join(", ") +
                        " WHERE id = @id;";
                rCommand.Parameters.AddWithValue("@id", SortieInfo.Current.ID);

                rCommand.ExecuteNonQuery();

                rTransaction.Commit();
            }
        }
        string GetRewardSetter(MaterialType rpType, int rpQuantity)
        {
            switch (rpType)
            {
                case MaterialType.Fuel:
                    return "fuel = coalesce(fuel, 0) + " + rpQuantity;

                case MaterialType.Bullet:
                    return "bullet = coalesce(bullet, 0) + " + rpQuantity;

                case MaterialType.Steel:
                    return "steel = coalesce(steel, 0) + " + rpQuantity;

                case MaterialType.Bauxite:
                    return "bauxite = coalesce(bauxite, 0) + " + rpQuantity;

                case MaterialType.Bucket:
                    return "bucket = coalesce(bucket, 0) + " + rpQuantity;

                default:
                    return null;
            }
        }

        void StartPractice(ApiInfo rpInfo)
        {
            var rFleet = Port.Fleets[int.Parse(rpInfo.Parameters["api_deck_id"])];

            AddFleet(rpInfo.Timestamp, rFleet, ShipParticipantType.Practice);
        }

        void ExpeditionResult(ApiInfo rpInfo)
        {
            var rFleet = Port.Fleets[int.Parse(rpInfo.Parameters["api_deck_id"])];

            AddFleet(rpInfo.Timestamp, rFleet, ShipParticipantType.NormalExpedition);
        }

        void AddFleet(long rpTimestamp, Fleet rpFleet, ShipParticipantType rpType)
        {
            using (var rTransaction = Connection.BeginTransaction())
            using (var rCommand = Connection.CreateCommand())
            {
                var rBuilder = new StringBuilder(256);
                rBuilder.Append("INSERT INTO sortie_consumption(id) VALUES (@id); ");
                rBuilder.Append("INSERT INTO sortie_participant_ship(id, ship_id, ship, type) VALUES");

                var rCount = 0;
                foreach (var rShip in rpFleet.Ships)
                {
                    if (rCount++ > 0)
                        rBuilder.Append(", ");

                    rBuilder.Append($"(@id, {rShip.ID}, {rShip.Info.ID}, @type)");

                    rCount++;
                }
                rBuilder.Append(';');

                rCommand.CommandText = rBuilder.ToString();
                rCommand.Parameters.AddWithValue("@id", rpTimestamp);
                rCommand.Parameters.AddWithValue("@type", (int)rpType);

                rCommand.ExecuteNonQuery();

                rTransaction.Commit();
            }
        }

        struct SupplySnapshot
        {
            public bool IsMarried { get; set; }

            public int Fuel { get; }
            public int Bullet { get; }

            public int SlotCount { get; }
            public int Plane { get; }

            public SupplySnapshot(bool rpIsMarried, int rpFuel, int rpBullet, int rpSlotCount, int rpPlane)
            {
                IsMarried = rpIsMarried;

                Fuel = rpFuel;
                Bullet = rpBullet;
                Plane = rpPlane;

                SlotCount = rpSlotCount;
            }
        }

        struct AnchorageRepairSnapshot
        {
            public int ShipID { get; }

            Ship r_Ship;
            public Ship Ship
            {
                get
                {
                    if (r_Ship == null)
                        r_Ship = KanColleGame.Current.Port.Ships[ShipID];

                    return r_Ship;
                }
                set { r_Ship = value; }
            }

            public int HP { get; }

            public double RepairTime { get; }

            public int FuelConsumption { get; }
            public int SteelConsumption { get; }

            public AnchorageRepairSnapshot(int rpShipID, int rpHP, double rpRepairTime, int rpFuelConsumption, int rpSteelConsumption)
            {
                ShipID = rpShipID;
                r_Ship = null;

                HP = rpHP;

                RepairTime = rpRepairTime;

                FuelConsumption = rpFuelConsumption;
                SteelConsumption = rpSteelConsumption;
            }
            public AnchorageRepairSnapshot(Ship rpShip, int rpHP, double rpRepairTime, int rpFuelConsumption, int rpSteelConsumption)
            {
                ShipID = rpShip.ID;
                r_Ship = rpShip;

                HP = rpHP;

                RepairTime = rpRepairTime;

                FuelConsumption = rpFuelConsumption;
                SteelConsumption = rpSteelConsumption;
            }
        }
    }
}
