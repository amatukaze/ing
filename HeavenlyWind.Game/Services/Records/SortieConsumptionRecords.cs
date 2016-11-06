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
        public override int Version => 3;

        Port Port = KanColleGame.Current.Port;

        enum ShipParticipantType { Sortie, SupportFire, NormalExpedition, Practice }
        enum ConsumptionType { Supply, Repair, Remodel, AirBasePlaneDeployment, AirForceGroupSortie, AirForceSquadronSupply, EnemyRaid }

        ListDictionary<int, SupplySnapshot> r_SupplySnapshots = new ListDictionary<int, SupplySnapshot>();

        bool r_AnchorageRepairSnapshotsInitialized;
        long r_AnchorageRepairStartTime;
        ListDictionary<int, AnchorageRepairSnapshot> r_AnchorageRepairSnapshots = new ListDictionary<int, AnchorageRepairSnapshot>();

        AirForceGroup r_SupplyingGroup;
        AirForceSquadron[] r_SupplyingSquadrons;
        int r_TotalPlaneCount;

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

            DisposableObjects.Add(ApiService.Subscribe(new[] { "api_req_sortie/battleresult", "api_req_combined_battle/battleresult" }, ProcessBattleResult));
            DisposableObjects.Add(ApiService.Subscribe("api_port/port", CommitReward));
            DisposableObjects.Add(ApiService.Subscribe("api_start2", ForfeitReward));

            DisposableObjects.Add(ApiService.Subscribe("api_req_practice/battle", StartPractice));

            DisposableObjects.Add(ApiService.Subscribe("api_req_mission/result", ExpeditionResult));

            DisposableObjects.Add(ApiService.Subscribe("api_req_air_corps/set_plane", AirBasePlaneDeployment));
            DisposableObjects.Add(ApiService.Subscribe("api_req_air_corps/supply", BeforeAirForceSquadronSupply, AfterAirForceSquadronSupply));
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
                    "CREATE TABLE IF NOT EXISTS sortie_participant_airbase(" +
                        "id INTEGER NOT NULL REFERENCES sortie_consumption(id) ON DELETE CASCADE ON UPDATE CASCADE, " +
                        "[group] INTEGER NOT NULL, " +
                        "plane_id INTEGER NOT NULL, " +
                        "plane INTEGER NOT NULL, " +
                        "PRIMARY KEY(id, [group], plane_id)) WITHOUT ROWID; " +

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

                    "CREATE TABLE IF NOT EXISTS airbase_plane_deployment_consumption(" +
                        "area INTEGER NOT NULL, " +
                        "[group] INTEGER NOT NULL, " +
                        "bauxite INTEGER NOT NULL, " +
                        "PRIMARY KEY(area, [group])); " +

                    "CREATE TABLE IF NOT EXISTS sortie_reward(" +
                        "id INTEGER PRIMARY KEY NOT NULL REFERENCES sortie_consumption(id) ON DELETE CASCADE ON UPDATE CASCADE, " +
                        "fuel INTEGER, " +
                        "bullet INTEGER, " +
                        "steel INTEGER, " +
                        "bauxite INTEGER, " +
                        "bucket INTEGER); " +

                    "CREATE TEMPORARY TABLE IF NOT EXISTS sortie_reward_pending(" +
                        "type INTEGER PRIMARY KEY NOT NULL, " +
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
            var rBuilder = new StringBuilder(512);
            rBuilder.AppendLine("DELETE FROM sortie_reward_pending;");
            rBuilder.AppendLine("INSERT INTO sortie_consumption(id) VALUES (@id);");
            rBuilder.Append("INSERT INTO sortie_participant_ship(id, ship_id, ship, type) VALUES");

            var rSortie = SortieInfo.Current;
            var rCommand = Connection.CreateCommand();

            ProcessParticipants(rBuilder, rSortie);
            ProcessSupportFleets(rBuilder);

            rBuilder.AppendLine(";");

            if (rSortie.Map.AvailableAirBaseGroupCount > 0)
                ProcessLBASConsumption(rBuilder, rCommand.Parameters, rSortie);

            rCommand.CommandText = rBuilder.ToString();
            rCommand.Parameters.AddWithValue("@id", rSortie.ID);

            rCommand.PostToTransactionQueue();
        }
        void ProcessParticipants(StringBuilder rpBuilder, SortieInfo rpSortie)
        {
            IEnumerable<IParticipant> rParticipants = rpSortie.MainShips;
            if (rpSortie.EscortShips != null)
                rParticipants = rParticipants.Concat(rpSortie.EscortShips);

            var rFirst = true;
            foreach (FriendShip rParticipant in rParticipants)
            {
                if (rFirst)
                    rFirst = false;
                else
                    rpBuilder.Append(", ");

                rpBuilder.Append($"(@id, {rParticipant.Ship.ID}, {rParticipant.Info.ID}, 0)");
            }
        }
        void ProcessSupportFleets(StringBuilder rpBuilder)
        {
            var rSupportFleets = Port.Fleets.Table.Values
                .Where(r =>
                {
                    var rExpedition = r.ExpeditionStatus.Expedition;

                    return rExpedition != null && !rExpedition.CanReturn && rExpedition.MapArea.ID == SortieInfo.Current.Map.MasterInfo.AreaID;
                }).ToArray();

            if (rSupportFleets.Length > 0)
                foreach (var rShip in rSupportFleets.SelectMany(r => r.Ships))
                    rpBuilder.Append($", (@id, {rShip.ID}, {rShip.Info.ID}, 1)");
        }
        void ProcessLBASConsumption(StringBuilder rpBuilder, SQLiteParameterCollection rpParameters, SortieInfo rpSortie)
        {
            var rMap = rpSortie.Map;
            var rAllGroups = Port.AirBase.Table[rMap.MasterInfo.AreaID].Values;
            var rGroups = rAllGroups.Take(rMap.AvailableAirBaseGroupCount).Where(r => r.Option == AirForceGroupOption.Sortie)
                .Concat(rAllGroups.Where(r => r.Option == AirForceGroupOption.AirDefense))
                .ToArray();

            if (rGroups.Length == 0)
                return;

            ProcessAirBaseSquadronParticipants(rpBuilder, rGroups);
            ProcessAirBasePlaneDeploymentConsumption(rpBuilder, rpParameters, rMap, rGroups);
            ProcessAirForceGroupSortieConsumption(rpBuilder, rpParameters, rGroups);
        }
        void ProcessAirBaseSquadronParticipants(StringBuilder rpBuilder, AirForceGroup[] rpGroups)
        {
            rpBuilder.Append("INSERT INTO sortie_participant_airbase(id, [group], plane_id, plane) VALUES");

            var rFirst = true;
            foreach (var rGroup in rpGroups)
                foreach (var rSquadron in rGroup.Squadrons.Values)
                {
                    if (rFirst)
                        rFirst = false;
                    else
                        rpBuilder.Append(", ");

                    rpBuilder.Append($"(@id, {rGroup.ID}, {rSquadron.Plane.ID}, {rSquadron.Plane.Info.ID})");
                }

            rpBuilder.AppendLine(";");
        }

        void ProcessAirBasePlaneDeploymentConsumption(StringBuilder rpBuilder, SQLiteParameterCollection rpParameters, MapInfo rpMap, AirForceGroup[] rpGroups)
        {
            var rStatement = string.Join(", ", rpGroups.Select(r => r.ID));

            rpBuilder.AppendFormat("INSERT OR IGNORE INTO sortie_consumption_detail(id, type, bauxite) VALUES(@id, 3, (SELECT sum(bauxite) FROM airbase_plane_deployment_consumption WHERE area = @area AND [group] IN ({0})));{1}" +
                "DELETE FROM sortie_consumption_detail WHERE id = @id AND type = 3 AND bauxite IS NULL;{1}" +
                "DELETE FROM airbase_plane_deployment_consumption WHERE area = @area AND [group] IN ({0});", rStatement, Environment.NewLine);
            rpBuilder.AppendLine();
            rpParameters.AddWithValue("@area", rpMap.MasterInfo.AreaID);
            rpParameters.AddWithValue("@group", rpMap.AvailableAirBaseGroupCount);
        }
        void ProcessAirForceGroupSortieConsumption(StringBuilder rpBuilder, SQLiteParameterCollection rpParameters, AirForceGroup[] rpGroups)
        {
            rpBuilder.AppendLine("INSERT OR IGNORE INTO sortie_consumption_detail(id, type, fuel, bullet) VALUES(@id, 4, @afgs_fuel, @afgs_bullet);");
            rpParameters.AddWithValue("@afgs_fuel", rpGroups.Sum(r => r.LBASFuelConsumption));
            rpParameters.AddWithValue("@afgs_bullet", rpGroups.Sum(r => r.LBASBulletConsumption));
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

            var rBuilder = new StringBuilder(256);
            rBuilder.Append("INSERT OR IGNORE INTO sortie_consumption_detail(id, type) VALUES");

            var rFirst = true;
            foreach (var rShipSupplyResult in rData.Ships)
            {
                if (rFirst)
                    rFirst = false;
                else
                    rBuilder.Append(", ");

                rBuilder.Append($"((SELECT max(id) FROM sortie_participant_ship WHERE ship_id = {rShipSupplyResult.ID}), 0)");
            }
            rBuilder.AppendLine("; ");

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

                rBuilder.AppendLine($"UPDATE sortie_consumption_detail SET fuel = coalesce(fuel, 0) + {rFuelDiff}, bullet = coalesce(bullet, 0) + {rBulletDiff}, bauxite = coalesce(bauxite, 0) + {rBauxiteDiff} WHERE id = (SELECT max(id) FROM sortie_participant_ship WHERE ship_id = {rShipSupplyResult.ID}) AND type = 0; ");
            }

            r_SupplySnapshots.Clear();

            var rCommand = Connection.CreateCommand();
            rCommand.CommandText = rBuilder.ToString();

            rCommand.PostToTransactionQueue();
        }

        void Repair(ApiInfo rpInfo)
        {
            var rShip = Port.Ships[int.Parse(rpInfo.Parameters["api_ship_id"])];
            var rUseBucket = rpInfo.Parameters["api_highspeed"] == "1";

            var rCommand = Connection.CreateCommand();
            rCommand.CommandText =
                "INSERT OR IGNORE INTO sortie_consumption_detail(id, type) VALUES((SELECT max(id) FROM sortie_participant_ship WHERE ship_id = @ship_id AND type = 0), 1);" +
                "UPDATE sortie_consumption_detail SET fuel = coalesce(fuel, 0) + @fuel, steel = coalesce(steel, 0) + @steel, bucket = coalesce(bucket, 0) + @bucket WHERE id = (SELECT max(id) FROM sortie_participant_ship WHERE ship_id = @ship_id AND type = 0) AND type = 1;";
            rCommand.Parameters.AddWithValue("@ship_id", rShip.ID);
            rCommand.Parameters.AddWithValue("@fuel", rShip.RepairFuelConsumption);
            rCommand.Parameters.AddWithValue("@steel", rShip.RepairSteelConsumption);
            rCommand.Parameters.AddWithValue("@bucket", rUseBucket ? 1 : 0);

            rCommand.PostToTransactionQueue();
        }
        void UseBucket(ApiInfo rpInfo)
        {
            var rDock = Port.RepairDocks[int.Parse(rpInfo.Parameters["api_ndock_id"])];

            var rCommand = Connection.CreateCommand();
            rCommand.CommandText =
                "INSERT OR IGNORE INTO sortie_consumption_detail(id, type) VALUES((SELECT max(id) FROM sortie_participant_ship WHERE ship_id = @ship_id AND type = 0), 1);" +
                "UPDATE sortie_consumption_detail SET bucket = coalesce(bucket, 0) + 1 WHERE id = (SELECT max(id) FROM sortie_participant_ship WHERE ship_id = @ship_id AND type = 0) AND type = 1;";
            rCommand.Parameters.AddWithValue("@ship_id", rDock.Ship.ID);

            rCommand.PostToTransactionQueue();
        }

        void ResetAnchorageRepairSnapshots(ApiInfo rpInfo)
        {
            var rBuilder = new StringBuilder(128);
            rBuilder.AppendLine("DELETE FROM anchorage_repair; ");
            rBuilder.AppendLine("DELETE FROM common WHERE key = 'anchorage_repair_start_time'; ");

            r_AnchorageRepairSnapshots.Clear();

            var rShips = Port.Fleets.Table.Values.SelectMany(r => r.AnchorageRepair.RepairingShips).ToArray();
            if (rShips.Length == 0)
                r_AnchorageRepairStartTime = 0;
            else
            {
                r_AnchorageRepairStartTime = rpInfo.Timestamp;

                rBuilder.Append("INSERT INTO anchorage_repair(ship, hp, repair_time, fuel_consumption, steel_consumption) VALUES");

                var rFirst = true;
                foreach (var rShip in rShips)
                {
                    if (rFirst)
                        rFirst = false;
                    else
                        rBuilder.Append(", ");

                    r_AnchorageRepairSnapshots.Add(rShip.ID, new AnchorageRepairSnapshot(rShip, rShip.HP.Current, rShip.RepairTime.Value.TotalMinutes, rShip.RepairFuelConsumption, rShip.RepairSteelConsumption));

                    rBuilder.Append($"({rShip.ID}, {rShip.HP.Current}, {rShip.RepairTime.Value.TotalMinutes}, {rShip.RepairFuelConsumption}, {rShip.RepairSteelConsumption})");
                }
                rBuilder.AppendLine(";");

                rBuilder.AppendLine($"INSERT OR REPLACE INTO common(key, value) VALUES('anchorage_repair_start_time', {r_AnchorageRepairStartTime});");
            }

            var rCommand = Connection.CreateCommand();
            rCommand.CommandText = rBuilder.ToString();

            rCommand.PostToTransactionQueue();
        }
        void ProcessAnchorageRepair(ApiInfo rpInfo)
        {
            InitializeAnchorageRepairSnapshots(rpInfo);

            var rSnapshots = r_AnchorageRepairSnapshots.Values.Where(r => r.Ship.HP.Current > r.HP).ToArray();
            if (rSnapshots.Length == 0)
                return;

            var rBuilder = new StringBuilder(512);
            rBuilder.Append("INSERT OR IGNORE INTO sortie_consumption_detail(id, type) VALUES");

            var rFirst = true;
            foreach (var rSnapshot in rSnapshots)
            {
                if (rFirst)
                    rFirst = false;
                else
                    rBuilder.Append(", ");

                rBuilder.Append($"((SELECT max(id) FROM sortie_participant_ship WHERE ship_id = {rSnapshot.ShipID}), 4)");
            }
            rBuilder.AppendLine(";");

            var rRemovedIDs = new List<int>();

            var rTimeDiff = Math.Ceiling((rpInfo.Timestamp - r_AnchorageRepairStartTime) / 60.0 / 20.0);

            foreach (var rSnapshot in rSnapshots)
            {
                var rRate = Math.Min(1.0, rTimeDiff / Math.Ceiling(rSnapshot.RepairTime / 20.0));

                var rFuelConsumption = Math.Ceiling(rSnapshot.FuelConsumption * rRate);
                var rSteelConsumption = Math.Ceiling(rSnapshot.SteelConsumption * rRate);

                rBuilder.AppendLine($"UPDATE sortie_consumption_detail SET fuel = ifnull(fuel, 0) + {rFuelConsumption}, steel = ifnull(steel, 0) + {rSteelConsumption} WHERE id = (SELECT max(id) FROM sortie_participant_ship WHERE ship_id = {rSnapshot.ShipID}) AND type = 4; ");

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
                rBuilder.AppendLine("DELETE FROM anchorage_repair WHERE ship IN (" + string.Join(", ", rRemovedIDs) + "); ");

            if (r_AnchorageRepairSnapshots.Count == 0)
            {
                r_AnchorageRepairStartTime = 0;
                rBuilder.AppendLine("DELETE FROM common WHERE key = 'anchorage_repair_start_time';");
            }
            else
            {
                r_AnchorageRepairStartTime = rpInfo.Timestamp;
                rBuilder.AppendLine($"INSERT OR REPLACE INTO common(key, value) VALUES('anchorage_repair_start_time', {r_AnchorageRepairStartTime});");

                rBuilder.AppendLine("INSERT OR REPLACE INTO anchorage_repair(ship, hp, repair_time, fuel_consumption, steel_consumption) VALUES");

                rFirst = true;
                foreach (var rSnapshot in r_AnchorageRepairSnapshots.Values)
                {
                    if (rFirst)
                        rFirst = false;
                    else
                        rBuilder.Append(", ");

                    rBuilder.Append($"({rSnapshot.ShipID}, {rSnapshot.HP}, {rSnapshot.RepairTime}, {rSnapshot.FuelConsumption}, {rSnapshot.SteelConsumption})");
                }
                rBuilder.AppendLine(";");
            }

            var rCommand = Connection.CreateCommand();
            rCommand.CommandText = rBuilder.ToString();
            rCommand.PostToTransactionQueue();
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

            var rCommand = Connection.CreateCommand();

            if (rAbsentShipIDs.Length > 0)
            {
                rCommand.CommandText = "DELETE FROM anchorage_repair WHERE ship IN (" + string.Join(", ", rAbsentShipIDs) + ");";

                foreach (var rID in rAbsentShipIDs)
                    r_AnchorageRepairSnapshots.Remove(rID);
            }

            foreach (var rShip in rRepairingShips.Where(r => !r_AnchorageRepairSnapshots.ContainsKey(r.ID)))
                r_AnchorageRepairSnapshots[rShip.ID] = new AnchorageRepairSnapshot(rShip, rShip.HP.Current, rShip.RepairTime.Value.TotalMinutes, rShip.RepairFuelConsumption, rShip.RepairSteelConsumption);

            if (r_AnchorageRepairSnapshots.Count == 0)
            {
                r_AnchorageRepairStartTime = 0;

                rCommand.CommandText += "DELETE FROM common WHERE key = 'anchorage_repair_start_time';";
            }

            r_AnchorageRepairSnapshotsInitialized = true;

            rCommand.PostToTransactionQueue();
        }

        void Remodel(ApiInfo rpInfo)
        {
            var rShip = Port.Ships[int.Parse(rpInfo.Parameters["api_id"])];

            var rCommand = Connection.CreateCommand();
            rCommand.CommandText =
                "INSERT OR IGNORE INTO sortie_consumption_detail(id, type) VALUES((SELECT max(id) FROM sortie_participant_ship WHERE ship_id = @ship_id), 2);" +
                "UPDATE sortie_consumption_detail SET bullet = coalesce(bullet, 0) + @bullet, steel = coalesce(steel, 0) + @steel WHERE id = (SELECT max(id) FROM sortie_participant_ship WHERE ship_id = @ship_id) AND type = 2;";
            rCommand.Parameters.AddWithValue("@ship_id", rShip.ID);
            rCommand.Parameters.AddWithValue("@bullet", rShip.Info.RemodelingFuelConsumption);
            rCommand.Parameters.AddWithValue("@steel", rShip.Info.RemodelingBulletConsumption);

            rCommand.PostToTransactionQueue();
        }

        void Exploration(ApiInfo rpInfo)
        {
            var rCommand = Connection.CreateCommand();
            rCommand.CommandText = "INSERT OR REPLACE INTO common(key, value) VALUES('is_reward_forfeited', @forfeited);";
            rCommand.Parameters.AddWithValue("@id", SortieInfo.Current.ID);

            var rNode = SortieInfo.Current.Node;
            rCommand.Parameters.AddWithValue("@forfeited", !rNode.IsDeadEnd);

            var rEvent = rNode.Event as RewardEventBase;
            if (rEvent != null)
                ProcessReward(rCommand, rNode, rEvent);

            rCommand.PostToTransactionQueue();
        }
        void ProcessReward(SQLiteCommand rpCommand, SortieNodeInfo rpNode, RewardEventBase rpEvent)
        {
            StringBuilder rBuilder = null;

            switch (rpNode.EventType)
            {
                case SortieEventType.Reward:
                    var rRewards = ((RewardEvent)rpEvent).Rewards;
                    if (rRewards == null || rRewards.Count == 0)
                        return;

                    foreach (var rReward in rRewards)
                    {
                        if (!IsRewardValid(rReward.ID))
                            continue;

                        if (rBuilder == null)
                        {
                            rBuilder = new StringBuilder(256);
                            rBuilder.AppendLine("INSERT OR IGNORE INTO sortie_reward_pending(type) VALUES(0);");
                        }

                        var rSetter = GetRewardSetter(rReward.ID, rReward.Quantity);

                        rBuilder.Append("UPDATE sortie_reward_pending SET ");
                        rBuilder.Append(rSetter);
                        rBuilder.AppendLine(" WHERE type = 0;");
                    }
                    break;

                case SortieEventType.AviationReconnaissance:
                    if (((AviationReconnaissanceEvent)rpEvent).Result == AviationReconnaissanceResult.Failure || !IsRewardValid(rpEvent.ID))
                        return;

                    rBuilder = new StringBuilder(256);
                    rBuilder.AppendLine("INSERT OR IGNORE INTO sortie_reward_pending(type) VALUES(1);");
                    rBuilder.Append("UPDATE sortie_reward_pending SET ");
                    rBuilder.Append(GetRewardSetter(rpEvent.ID, rpEvent.Quantity));
                    rBuilder.AppendLine(" WHERE type = 1;");
                    break;

                case SortieEventType.EscortSuccess:
                    if (!IsRewardValid(rpEvent.ID))
                        return;

                    rBuilder = new StringBuilder(256);
                    rBuilder.AppendLine("INSERT OR IGNORE INTO sortie_reward(id) VALUES(@id);");
                    rBuilder.Append("UPDATE sortie_reward SET ");
                    rBuilder.Append(GetRewardSetter(rpEvent.ID, rpEvent.Quantity));
                    rBuilder.AppendLine(" WHERE id = @id;");
                    break;

                default: return;
            }

            if (rBuilder == null)
                return;

            rpCommand.CommandText += rBuilder.ToString();
        }
        bool IsRewardValid(MaterialType rpType) =>
            rpType == MaterialType.Fuel ||
            rpType == MaterialType.Bullet ||
            rpType == MaterialType.Steel ||
            rpType == MaterialType.Bauxite ||
            rpType == MaterialType.Bucket;
        string GetRewardSetter(MaterialType rpType, int rpQuantity)
        {
            switch (rpType)
            {
                case MaterialType.Fuel:
                    return "fuel = ifnull(fuel, 0) + " + rpQuantity;

                case MaterialType.Bullet:
                    return "bullet = ifnull(bullet, 0) + " + rpQuantity;

                case MaterialType.Steel:
                    return "steel = ifnull(steel, 0) + " + rpQuantity;

                case MaterialType.Bauxite:
                    return "bauxite = ifnull(bauxite, 0) + " + rpQuantity;

                case MaterialType.Bucket:
                    return "bucket = ifnull(bucket, 0) + " + rpQuantity;
            }

            throw new ArgumentException(nameof(rpType));
        }

        void ProcessBattleResult(ApiInfo rpInfo)
        {
            var rData = rpInfo.GetData<RawBattleResult>();

            var rCommand = RecordService.Instance.CreateCommand();
            rCommand.CommandText = "INSERT OR REPLACE INTO common(key, value) VALUES('is_reward_forfeited', (SELECT ifnull((SELECT fuel IS NULL AND bullet IS NULL AND steel IS NULL AND bauxite IS NULL AND bucket IS NULL FROM sortie_reward_pending WHERE type = 0), 1)));";

            if (rData.IsAviationReconnaissanceRewardConfirmed)
            {
                rCommand.CommandText +=
                    "INSERT OR IGNORE INTO sortie_reward(id) VALUES(@id); " +
                    "UPDATE sortie_reward SET fuel = nullif(ifnull(fuel, 0) + ifnull((SELECT fuel FROM sortie_reward_pending WHERE type = 1), 0), 0), " +
                        "bullet = nullif(ifnull(bullet, 0) + ifnull((SELECT bullet FROM sortie_reward_pending WHERE type = 1), 0), 0), " +
                        "steel = nullif(ifnull(steel, 0) + ifnull((SELECT steel FROM sortie_reward_pending WHERE type = 1), 0), 0), " +
                        "bauxite = nullif(ifnull(bauxite, 0) + ifnull((SELECT bauxite FROM sortie_reward_pending WHERE type = 1), 0), 0), " +
                        "bucket = nullif(ifnull(bucket, 0) + ifnull((SELECT bucket FROM sortie_reward_pending WHERE type = 1), 0), 0) WHERE id = @id; " +
                    "DELETE FROM sortie_reward_pending WHERE type = 1;";
                rCommand.Parameters.AddWithValue("@id", SortieInfo.Current.ID);
            }

            rCommand.PostToTransactionQueue();
        }
        void CommitReward(ApiInfo rpInfo)
        {
            var rCommand = RecordService.Instance.CreateCommand();
            rCommand.CommandText = "SELECT value FROM common WHERE key = 'is_reward_forfeited';";

            var rIsForfeited = rCommand.ExecuteScalar();
            if (rIsForfeited == DBNull.Value || Convert.ToBoolean(rIsForfeited))
                return;

            rCommand.CommandText =
                "INSERT OR IGNORE INTO sortie_reward(id) VALUES((SELECT max(id) FROM sortie)); " +
                "UPDATE sortie_reward SET fuel = nullif(ifnull(fuel, 0) + ifnull((SELECT fuel FROM sortie_reward_pending WHERE type = 0), 0), 0), " +
                    "bullet = nullif(ifnull(bullet, 0) + ifnull((SELECT bullet FROM sortie_reward_pending WHERE type = 0), 0), 0), " +
                    "steel = nullif(ifnull(steel, 0) + ifnull((SELECT steel FROM sortie_reward_pending WHERE type = 0), 0), 0), " +
                    "bauxite = nullif(ifnull(bauxite, 0) + ifnull((SELECT bauxite FROM sortie_reward_pending WHERE type = 0), 0), 0), " +
                    "bucket = nullif(ifnull(bucket, 0) + ifnull((SELECT bucket FROM sortie_reward_pending WHERE type = 0), 0), 0) WHERE id = (SELECT max(id) FROM sortie); " +
                "DELETE FROM sortie_reward_pending WHERE type = 0; " +
                "INSERT OR REPLACE INTO common(key, value) VALUES('is_reward_forfeited', 1);";

            rCommand.PostToTransactionQueue();
        }
        void ForfeitReward(ApiInfo rpInfo)
        {
            var rCommand = RecordService.Instance.CreateCommand();
            rCommand.CommandText = "INSERT OR REPLACE INTO common(key, value) VALUES('is_reward_forfeited', 1);";
            rCommand.ExecuteNonQuery();
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
            var rBuilder = new StringBuilder(256);
            rBuilder.Append("INSERT INTO sortie_consumption(id) VALUES (@id); ");
            rBuilder.Append("INSERT INTO sortie_participant_ship(id, ship_id, ship, type) VALUES");

            var rFirst = true;
            foreach (var rShip in rpFleet.Ships)
            {
                if (rFirst)
                    rFirst = false;
                else
                    rBuilder.Append(", ");

                rBuilder.Append($"(@id, {rShip.ID}, {rShip.Info.ID}, @type)");
            }
            rBuilder.Append(';');

            var rCommand = Connection.CreateCommand();
            rCommand.CommandText = rBuilder.ToString();
            rCommand.Parameters.AddWithValue("@id", rpTimestamp);
            rCommand.Parameters.AddWithValue("@type", (int)rpType);

            rCommand.PostToTransactionQueue();
        }

        void AirBasePlaneDeployment(ApiInfo rpInfo)
        {
            var rPlaneID = int.Parse(rpInfo.Parameters["api_item_id"]);
            if (rPlaneID == -1)
                return;

            var rData = rpInfo.GetData<RawAirForceGroupOrganization>();
            if (rData.Squadrons.Length > 1)
                return;

            var rPlane = Port.Equipment[rPlaneID].Info;

            var rArea = int.Parse(rpInfo.Parameters["api_area_id"]);
            var rGroup = int.Parse(rpInfo.Parameters["api_base_id"]);
            var rSquadron = int.Parse(rpInfo.Parameters["api_squadron_id"]);

            var rBauxite = rPlane.DeploymentBauxiteConsumption * rData.Squadrons[0].Count;

            var rCommand = Connection.CreateCommand();
            rCommand.CommandText =
                "INSERT OR IGNORE INTO airbase_plane_deployment_consumption(area, [group], bauxite) VALUES(@area, @group, 0); " +
                "UPDATE airbase_plane_deployment_consumption SET bauxite = bauxite + @bauxite WHERE area = @area AND [group] = @group;";
            rCommand.Parameters.AddWithValue("@area", rArea);
            rCommand.Parameters.AddWithValue("@group", rGroup);
            rCommand.Parameters.AddWithValue("@bauxite", rBauxite);

            rCommand.PostToTransactionQueue();
        }

        void BeforeAirForceSquadronSupply(ApiInfo rpInfo)
        {
            var rArea = int.Parse(rpInfo.Parameters["api_area_id"]);

            r_SupplyingGroup = Port.AirBase.Table[rArea][int.Parse(rpInfo.Parameters["api_base_id"])];
            r_SupplyingSquadrons = rpInfo.Parameters["api_squadron_id"].Split(',').Select(r => r_SupplyingGroup.Squadrons[int.Parse(r)]).ToArray();

            r_TotalPlaneCount = r_SupplyingSquadrons.Sum(r => r.Count);
        }
        void AfterAirForceSquadronSupply(ApiInfo rpInfo)
        {
            var rArea = int.Parse(rpInfo.Parameters["api_area_id"]);
            var rCountDiff = r_SupplyingSquadrons.Sum(r => r.Count) - r_TotalPlaneCount;

            var rBuilder = new StringBuilder(256);
            rBuilder.AppendLine("INSERT OR IGNORE INTO sortie_consumption_detail(id, type) VALUES((SELECT max(sortie.id) FROM sortie_participant_airbase JOIN sortie ON sortie.id = sortie_participant_airbase.id AND sortie.map / 10 = @area AND [group] = @group), 5);");
            rBuilder.AppendLine("UPDATE sortie_consumption_detail SET fuel = coalesce(fuel, 0) + @fuel, bauxite = coalesce(bauxite, 0) + @bauxite WHERE id = (SELECT max(sortie.id) FROM sortie_participant_airbase JOIN sortie ON sortie.id = sortie_participant_airbase.id AND sortie.map / 10 = @area AND [group] = @group) AND type = 5; ");

            var rCommand = Connection.CreateCommand();
            rCommand.CommandText = rBuilder.ToString();
            rCommand.Parameters.AddWithValue("@area", rArea);
            rCommand.Parameters.AddWithValue("@group", r_SupplyingGroup.ID);
            rCommand.Parameters.AddWithValue("@fuel", rCountDiff * 3);
            rCommand.Parameters.AddWithValue("@bauxite", rCountDiff * 5);

            r_SupplyingGroup = null;
            r_SupplyingSquadrons = null;

            rCommand.PostToTransactionQueue();
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
