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

        Port Port = KanColleGame.Current.Port;

        enum ShipParticipantType { Sortie, SupportFire, NormalExpedition, Practice }
        enum ConsumptionType { Supply, Repair, Remodel }

        ListDictionary<int, ShipSnapshot> r_Snapshots = new ListDictionary<int, ShipSnapshot>();

        public SortieConsumptionRecords(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(ApiService.Subscribe("api_req_map/start", StartSortie));

            DisposableObjects.Add(ApiService.Subscribe("api_req_hokyu/charge", BeforeSupply, AfterSupply));

            DisposableObjects.Add(ApiService.Subscribe("api_req_nyukyo/start", Repair));
            DisposableObjects.Add(ApiService.SubscribeOnlyOnBeforeProcessStarted("api_req_nyukyo/speedchange", UseBucket));

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

        void StartSortie(ApiInfo rpData)
        {
            var rSortie = SortieInfo.Current;

            using (var rTransaction = Connection.BeginTransaction())
            using (var rCommand = Connection.CreateCommand())
            {
                IEnumerable<IParticipant> rParticipants = rSortie.MainShips;
                if (rSortie.EscortShips != null)
                    rParticipants = rParticipants.Concat(rSortie.EscortShips);

                var rBuilder = new StringBuilder(128);
                rBuilder.Append("INSERT INTO sortie_consumption_detail(id) VALUES (@id); ");
                rBuilder.Append("INSERT INTO sortie_participant_ship(id, ship_id, ship, type) VALUES");

                var rCount = 0;
                foreach (FriendShip rParticipant in rParticipants)
                {
                    if (rCount++ > 0)
                        rBuilder.Append(", ");

                    rBuilder.Append($"(@id, {rParticipant.Ship.ID}, {rParticipant.Info.ID}, 0)");

                    rCount++;
                }
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

            r_Snapshots.Clear();

            foreach (var rShipSupplyResult in rData.Ships)
            {
                var rShip = Port.Ships[rShipSupplyResult.ID];
                var rPlaneCount = rShip.Slots.Take(rShip.Info.SlotCount).Sum(r => r.PlaneCount);

                r_Snapshots.Add(rShip.ID, new ShipSnapshot(rShip.IsMarried, rShip.Fuel.Current, rShip.Bullet.Current, rShip.Info.SlotCount, rPlaneCount));
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
                    var rSnapshot = r_Snapshots[rShipSupplyResult.ID];

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

            r_Snapshots.Clear();
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
                    "UPDATE sortie_consumption_detail SET bucket = coalesce(bucket, 0) + 1 WHERE id = (SELECT max(id) FROM sortie_participant_ship WHERE ship_id = @ship_id AND type = 0) AND type = 1;" +
                rCommand.Parameters.AddWithValue("@ship_id", rDock.Ship.ID);

                rCommand.ExecuteNonQuery();

                rTransaction.Commit();
            }
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
                var rRewards = new List<string>();

                switch (rNode.EventType)
                {
                    case SortieEventType.Reward:
                    case SortieEventType.AviationReconnaissance:
                        foreach (var rReward in ((RewardEvent)rEvent).Rewards)
                            if (rReward.TypeID == 4)
                                rRewards.Add(GetRewardString(rReward.ID, rReward.Quantity));
                        break;

                    case SortieEventType.EscortSuccess:
                        rRewards.Add(GetRewardString(rEvent.ID, rEvent.Quantity));
                        break;
                }

                rCommand.CommandText =
                    "INSERT OR IGNORE INTO sortie_reward(id) VALUES(@id);" +
                    "UPDATE sortie_reward SET " +
                        rRewards.Join(", ") +
                        " WHERE id = @id;";
                rCommand.Parameters.AddWithValue("@id", SortieInfo.Current.ID);

                rCommand.ExecuteNonQuery();

                rTransaction.Commit();
            }
        }
        string GetRewardString(MaterialType rpType, int rpQuantity)
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

                default: throw new InvalidOperationException();
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

        struct ShipSnapshot
        {
            public bool IsMarried { get; set; }

            public int Fuel { get; }
            public int Bullet { get; }

            public int SlotCount { get; }
            public int Plane { get; }

            public ShipSnapshot(bool rpIsMarried, int rpFuel, int rpBullet, int rpSlotCount, int rpPlane)
            {
                IsMarried = rpIsMarried;

                Fuel = rpFuel;
                Bullet = rpBullet;
                Plane = rpPlane;

                SlotCount = rpSlotCount;
            }
        }
    }
}
