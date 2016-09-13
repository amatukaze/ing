using Newtonsoft.Json.Linq;
using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class Port : ModelBase
    {
        public Admiral Admiral { get; private set; }

        public Materials Materials { get; } = new Materials();

        public HashSet<int> ShipIDs { get; private set; }
        public IDTable<Ship> Ships { get; } = new IDTable<Ship>();

        int[] r_EvacuatedShipIDs;
        internal HashSet<int> EvacuatedShipIDs { get; } = new HashSet<int>();

        public FleetManager Fleets { get; } = new FleetManager();

        public IDTable<Equipment> Equipment { get; } = new IDTable<Equipment>();

        static Regex r_UnequippedEquipmentRegex = new Regex(@"(?<=api_slottype)\d+", RegexOptions.Compiled);
        public IDictionary<EquipmentType, Equipment[]> UnequippedEquipment { get; } = new Dictionary<EquipmentType, Equipment[]>();

        public IDTable<RepairDock> RepairDocks { get; } = new IDTable<RepairDock>();
        public IDTable<ConstructionDock> ConstructionDocks { get; } = new IDTable<ConstructionDock>();

        public QuestManager Quests { get; } = new QuestManager();

        public AirBase AirBase { get; } = new AirBase();

        public event Action<IDictionary<EquipmentType, Equipment[]>> UnequippedEquipmentUpdated = delegate { };

        internal Port()
        {
            ApiService.Subscribe("api_get_member/ship2", r =>
            {
                UpdateShips(r.Json["api_data"].ToObject<RawShip[]>());
                Fleets.Update(r.Json["api_data_deck"].ToObject<RawFleet[]>());
            });
            ApiService.Subscribe("api_get_member/ship_deck", r =>
            {
                var rData = r.GetData<RawShipsAndFleets>();

                foreach (var rRawShip in rData.Ships)
                    Ships[rRawShip.ID].Update(rRawShip);

                var rRawFleet = rData.Fleets[0];
                Fleets[rRawFleet.ID].Update(rRawFleet);

                OnPropertyChanged(nameof(Ships));
            });
            ApiService.Subscribe("api_get_member/ship3", r =>
            {
                var rData = r.GetData<RawShipsAndFleets>();
                foreach (var rShip in rData.Ships)
                    Ships[rShip.ID].Update(rShip);
                Fleets.Update(rData.Fleets);

                ProcessUnequippedEquipment(r.Json["api_data"]["api_slot_data"]);
            });

            ApiService.Subscribe("api_get_member/unsetslot", r => ProcessUnequippedEquipment(r.Json["api_data"]));
            ApiService.Subscribe("api_get_member/require_info", r => ProcessUnequippedEquipment(r.Json["api_data"]["api_unsetslot"]));

            ApiService.Subscribe("api_req_kaisou/slot_exchange_index", r =>
            {
                Ship rShip;
                if (Ships.TryGetValue(int.Parse(r.Parameters["api_id"]), out rShip))
                {
                    rShip.UpdateEquipmentIDs(r.GetData<RawEquipmentIDs>().EquipmentIDs);
                    rShip.OwnerFleet?.Update();
                }
            });

            ApiService.Subscribe("api_req_kaisou/open_exslot", r =>
            {
                Ship rShip;
                if (Ships.TryGetValue(int.Parse(r.Parameters["api_id"]), out rShip))
                    rShip.InstallReinforcementExpansion();
            });

            ApiService.Subscribe("api_req_kaisou/slot_deprive", r =>
            {
                var rData = r.GetData<RawEquipmentRelocationResult>();

                var rDestionationShip = Ships[rData.Ships.Destination.ID];
                var rOriginShip = Ships[rData.Ships.Origin.ID];

                rDestionationShip.Update(rData.Ships.Destination);
                rOriginShip.Update(rData.Ships.Origin);

                rDestionationShip.OwnerFleet?.Update();
                rOriginShip.OwnerFleet?.Update();

                if (rData.UnequippedEquipment != null)
                    UnequippedEquipment[(EquipmentType)rData.UnequippedEquipment.Type] = rData.UnequippedEquipment.IDs.Select(rpID => Equipment[rpID]).ToArray();
            });

            ApiService.Subscribe("api_req_kaisou/powerup", r =>
            {
                var rShipID = int.Parse(r.Parameters["api_id"]);
                var rData = r.GetData<RawModernizationResult>();

                var rConsumedShips = r.Parameters["api_id_items"].Split(',').Select(rpID => Ships[int.Parse(rpID)]).ToArray();
                var rConsumedEquipment = rConsumedShips.SelectMany(rpShip => rpShip.EquipedEquipment).ToArray();

                foreach (var rEquipment in rConsumedEquipment)
                    Equipment.Remove(rEquipment);
                foreach (var rShip in rConsumedShips)
                    Ships.Remove(rShip);

                RemoveEquipmentFromUnequippedList(rConsumedEquipment);

                UpdateShipsCore();
                OnPropertyChanged(nameof(Equipment));
                Fleets.Update(rData.Fleets);

                Ship rModernizedShip;
                if (Ships.TryGetValue(rShipID, out rModernizedShip))
                {
                    if (!rData.Success)
                        Logger.Write(LoggingLevel.Info, string.Format(StringResources.Instance.Main.Log_Modernization_Failure, rModernizedShip.Info.TranslatedName));
                    else
                    {
                        var rOriginal = rModernizedShip.RawData;
                        var rNow = rData.Ship;
                        var rInfo = rModernizedShip.Info;

                        var rFirepowerDiff = rNow.Firepower[0] - rOriginal.Firepower[0];
                        var rTorpedoDiff = rNow.Torpedo[0] - rOriginal.Torpedo[0];
                        var rAADiff = rNow.AA[0] - rOriginal.AA[0];
                        var rArmorDiff = rNow.Armor[0] - rOriginal.Armor[0];
                        var rLuckDiff = rNow.Luck[0] - rOriginal.Luck[0];

                        var rDifferences = new List<string>(5);

                        if (rFirepowerDiff > 0)
                            if (rNow.Firepower[0] != rInfo.FirepowerMaximum)
                                rDifferences.Add($"[icon]firepower[/icon] {rOriginal.Firepower[0]}[b]+{rFirepowerDiff}[/b]/{rInfo.FirepowerMaximum}");
                            else
                                rDifferences.Add("[icon]firepower[/icon] MAX");
                        if (rTorpedoDiff > 0)
                            if (rNow.Torpedo[0] != rInfo.TorpedoMaximum)
                                rDifferences.Add($"[icon]torpedo[/icon] {rOriginal.Torpedo[0]}[b]+{rTorpedoDiff}[/b]/{rInfo.TorpedoMaximum}");
                            else
                                rDifferences.Add("[icon]torpedo[/icon] MAX");
                        if (rAADiff > 0)
                            if (rNow.AA[0] != rInfo.AAMaximum)
                                rDifferences.Add($"[icon]aa[/icon] {rOriginal.AA[0]}[b]+{rAADiff}[/b]/{rInfo.AAMaximum}");
                            else
                                rDifferences.Add("[icon]aa[/icon] MAX");
                        if (rArmorDiff > 0)
                            if (rNow.Armor[0] != rInfo.ArmorMaximum)
                                rDifferences.Add($"[icon]armor[/icon] {rOriginal.Armor[0]}[b]+{rArmorDiff}[/b]/{rInfo.ArmorMaximum}");
                            else
                                rDifferences.Add("[icon]armor[/icon] MAX");
                        if (rLuckDiff > 0)
                            if (rNow.Luck[0] != rInfo.LuckMaximum)
                                rDifferences.Add($"[icon]luck[/icon] {rOriginal.Luck[0]}[b]+{rLuckDiff}[/b]/{rInfo.LuckMaximum}");
                            else
                                rDifferences.Add("[icon]luck[/icon] MAX");

                        var rMessage = StringResources.Instance.Main.Log_Modernization_Success;
                        if (rDifferences.Count > 0)
                            rMessage += " (" + rDifferences.Join(" ") + ")";

                        Logger.Write(LoggingLevel.Info, string.Format(rMessage, rModernizedShip.Info.TranslatedName));
                    }

                    rModernizedShip.Update(rData.Ship);
                }
            });

            ApiService.Subscribe("api_req_kousyou/createship", r => ConstructionDocks[int.Parse(r.Parameters["api_kdock_id"])].IsConstructionStarted = true);
            ApiService.Subscribe("api_req_kousyou/getship", r =>
            {
                var rData = r.GetData<RawConstructionResult>();

                UpdateConstructionDocks(rData.ConstructionDocks);
                AddEquipment(rData.Equipment);

                Ships.Add(new Ship(rData.Ship));
                UpdateShipsCore();
            });
            ApiService.Subscribe("api_req_kousyou/createship_speedchange", r =>
            {
                if (r.Parameters["api_highspeed"] == "1")
                {
                    var rDock = ConstructionDocks[int.Parse(r.Parameters["api_kdock_id"])];
                    if (!rDock.IsLargeShipConstruction.GetValueOrDefault())
                        Materials.InstantConstruction--;
                    else
                        Materials.InstantConstruction -= 10;

                    rDock.CompleteConstruction();
                }
            });

            ApiService.Subscribe("api_req_kousyou/createitem", r =>
            {
                var rData = r.GetData<RawEquipmentDevelopment>();
                if (!rData.Success)
                    return;

                UnequippedEquipment[(EquipmentType)rData.EquipmentType] = r.Json["api_data"]["api_unsetslot"].Select(rpID => Equipment[(int)rpID]).ToArray();
            });

            ApiService.Subscribe("api_req_kousyou/destroyship", r =>
            {
                Materials.Update(r.Json["api_data"]["api_material"].ToObject<int[]>());

                var rShip = Ships[int.Parse(r.Parameters["api_ship_id"])];

                foreach (var rEquipment in rShip.EquipedEquipment)
                    Equipment.Remove(rEquipment);
                OnPropertyChanged(nameof(Equipment));

                RemoveEquipmentFromUnequippedList(rShip.EquipedEquipment);

                rShip.OwnerFleet?.Remove(rShip);
                Ships.Remove(rShip);
                UpdateShipsCore();
            });
            ApiService.Subscribe("api_req_kousyou/destroyitem2", r =>
            {
                var rEquipmentIDs = r.Parameters["api_slotitem_ids"].Split(',').Select(int.Parse);

                foreach (var rEquipmentID in rEquipmentIDs)
                    Equipment.Remove(rEquipmentID);
                OnPropertyChanged(nameof(Equipment));

                var rMaterials = r.Json["api_data"]["api_get_material"].ToObject<int[]>();
                Materials.Fuel += rMaterials[0];
                Materials.Bullet += rMaterials[1];
                Materials.Steel += rMaterials[2];
                Materials.Bauxite += rMaterials[3];
            });

            ApiService.Subscribe("api_req_kousyou/remodel_slotlist_detail", r =>
            {
                var rID = r.Parameters["api_slot_id"];
                var rEquipment = Equipment[int.Parse(rID)];

                Logger.Write(LoggingLevel.Info, string.Format(StringResources.Instance.Main.Log_EquipmentImprovement_Ready, rEquipment.Info.TranslatedName, rEquipment.LevelText, rID));
            });
            ApiService.Subscribe("api_req_kousyou/remodel_slot", r =>
            {
                var rData = (RawImprovementResult)r.Data;
                var rEquipment = Equipment[int.Parse(r.Parameters["api_slot_id"])];

                Materials.Update(rData.Materials);

                if (!rData.Success)
                    Logger.Write(LoggingLevel.Info, string.Format(StringResources.Instance.Main.Log_EquipmentImprovement_Failure, rEquipment.Info.TranslatedName, rEquipment.LevelText));
                else
                {
                    var rUpgraded = rEquipment.Info.ID != rData.ImprovedEquipment.EquipmentID;

                    rEquipment.Update(rData.ImprovedEquipment);

                    if (!rUpgraded)
                        Logger.Write(LoggingLevel.Info, string.Format(StringResources.Instance.Main.Log_EquipmentImprovement_Success, rEquipment.Info.TranslatedName, rEquipment.LevelText));
                    else
                        Logger.Write(LoggingLevel.Info, string.Format(StringResources.Instance.Main.Log_EquipmentImprovement_Upgrade, rEquipment.Info.TranslatedName, rEquipment.LevelText));
                }

                if (rData.ConsumedEquipmentID != null)
                {
                    var rConsumedEquipment = rData.ConsumedEquipmentID.Select(rpID => Equipment[rpID]).ToArray();

                    foreach (var rEquipmentID in rData.ConsumedEquipmentID)
                        Equipment.Remove(rEquipmentID);
                    OnPropertyChanged(nameof(Equipment));

                    RemoveEquipmentFromUnequippedList(rConsumedEquipment);
                }
            });

            ApiService.Subscribe("api_req_hokyu/charge", r =>
            {
                var rData = r.GetData<RawSupplyResult>();
                var rFleets = new HashSet<Fleet>();

                Materials.Update(rData.Materials);

                foreach (var rShipSupplyResult in rData.Ships)
                {
                    var rShip = Ships[rShipSupplyResult.ID];
                    rShip.Fuel.Current = rShipSupplyResult.Fuel;
                    rShip.Bullet.Current = rShipSupplyResult.Bullet;

                    if (rShip.OwnerFleet != null)
                        rFleets.Add(rShip.OwnerFleet);

                    var rPlanes = rShipSupplyResult.Planes;
                    for (var i = 0; i < rShip.Slots.Count; i++)
                    {
                        var rCount = rPlanes[i];
                        if (rCount > 0)
                            rShip.Slots[i].PlaneCount = rCount;
                    }

                    rShip.CombatAbility.Update();
                }

                foreach (var rFleet in rFleets)
                    rFleet.Update();
            });
            ApiService.Subscribe("api_req_air_corps/supply", r =>
            {
                var rData = r.GetData<RawAirForceSquadronResupplyResult>();

                Materials.Fuel = rData.Fuel;
                Materials.Bauxite = rData.Bauxite;
            });

            ApiService.Subscribe("api_get_member/ndock", r => UpdateRepairDocks(r.GetData<RawRepairDock[]>()));
            ApiService.Subscribe("api_req_nyukyo/start", r =>
            {
                var rShip = Ships[int.Parse(r.Parameters["api_ship_id"])];
                var rIsInstantRepair = r.Parameters["api_highspeed"] == "1";
                rShip.Repair(rIsInstantRepair);
                rShip.OwnerFleet?.Update();

                var rDock = RepairDocks[int.Parse(r.Parameters["api_ndock_id"])];
                rDock.PendingToUpdateMaterials = true;
                if (rIsInstantRepair)
                    Materials.Bucket--;
            });
            ApiService.Subscribe("api_req_nyukyo/speedchange", r =>
            {
                Materials.Bucket--;
                RepairDocks[int.Parse(r.Parameters["api_ndock_id"])].CompleteRepair();
            });

            ApiService.Subscribe("api_req_combined_battle/battleresult", r =>
            {
                var rData = (RawBattleResult)r.Data;
                if (!rData.HasEvacuatedShip)
                    return;

                var rEvacuatedShipIndex = rData.EvacuatedShips.EvacuatedShipIndex[0] - 1;
                var rEvacuatedShipID = Fleets[rEvacuatedShipIndex < 6 ? 1 : 2].Ships[rEvacuatedShipIndex % 6].ID;

                var rEscortShipIndex = rData.EvacuatedShips.EscortShipIndex[0] - 1;
                var rEscortShipID = Fleets[rEscortShipIndex < 6 ? 1 : 2].Ships[rEscortShipIndex % 6].ID;

                r_EvacuatedShipIDs = new[] { rEvacuatedShipID, rEscortShipID };
            });
            ApiService.Subscribe("api_req_combined_battle/goback_port", delegate
            {
                if (SortieInfo.Current == null || r_EvacuatedShipIDs == null || r_EvacuatedShipIDs.Length == 0)
                    return;

                EvacuatedShipIDs.Add(r_EvacuatedShipIDs[0]);
                EvacuatedShipIDs.Add(r_EvacuatedShipIDs[1]);
            });
            ApiService.Subscribe("api_get_member/ship_deck", _ => r_EvacuatedShipIDs = null);
            ApiService.Subscribe("api_port/port", _ => EvacuatedShipIDs.Clear());

            ApiService.Subscribe("api_req_member/updatedeckname", r =>
            {
                var rFleet = Fleets[int.Parse(r.Parameters["api_deck_id"])];
                rFleet.Name = r.Parameters["api_name"];
            });

            ApiService.Subscribe("api_req_mission/return_instruction", r =>
            {
                var rFleet = Fleets[int.Parse(r.Parameters["api_deck_id"])];
                rFleet.ExpeditionStatus.Update(r.GetData<RawExpeditionRecalling>().Expedition);
            });

            ApiService.Subscribe("api_req_air_corps/set_plane", r => Materials.Bauxite = r.GetData<RawAirForceGroupOrganization>().Bauxite);

            ApiService.Subscribe("api_req_hensei/lock", r => Ships[int.Parse(r.Parameters["api_ship_id"])].IsLocked = (bool)r.Json["api_data"]["api_locked"]);
            ApiService.Subscribe("api_req_kaisou/lock", r => Equipment[int.Parse(r.Parameters["api_slotitem_id"])].IsLocked = (bool)r.Json["api_data"]["api_locked"]);
        }

        #region Update

        internal void UpdateAdmiral(RawBasic rpAdmiral)
        {
            if (Admiral != null)
                Admiral.Update(rpAdmiral);
            else
            {
                Admiral = new Admiral(rpAdmiral);
                OnPropertyChanged(nameof(Admiral));
            }
        }

        internal void UpdatePort(RawPort rpPort)
        {
            UpdateAdmiral(rpPort.Basic);
            Materials.Update(rpPort.Materials);

            UpdateShips(rpPort);
            UpdateRepairDocks(rpPort.RepairDocks);

            Fleets.Update(rpPort);
        }

        internal void UpdateShips(RawPort rpPort) => UpdateShips(rpPort.Ships);
        internal void UpdateShips(RawShip[] rpShips)
        {
            if (Ships.UpdateRawData(rpShips, r => new Ship(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
                UpdateShipsCore();
        }

        internal void UpdateShipsCore()
        {
            ShipIDs = new HashSet<int>(Ships.Values.Select(r => r.Info.ID));
            OnPropertyChanged(nameof(Ships));
        }

        internal void UpdateEquipment(RawEquipment[] rpEquipment)
        {
            if (Equipment.UpdateRawData(rpEquipment, r => new Equipment(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
                OnPropertyChanged(nameof(Equipment));
        }
        internal void AddEquipment(Equipment rpEquipment)
        {
            Equipment.Add(rpEquipment);
            OnPropertyChanged(nameof(Equipment));
        }
        internal void AddEquipment(RawEquipment[] rpRawData)
        {
            if (rpRawData == null)
                return;

            foreach (var rRawData in rpRawData)
                Equipment.Add(new Equipment(rRawData));
            OnPropertyChanged(nameof(Equipment));
        }

        internal void UpdateConstructionDocks(RawConstructionDock[] rpConstructionDocks)
        {
            if (ConstructionDocks.UpdateRawData(rpConstructionDocks, r => new ConstructionDock(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
                OnPropertyChanged(nameof(ConstructionDocks));
        }
        void UpdateRepairDocks(RawRepairDock[] rpDocks)
        {
            if (RepairDocks.UpdateRawData(rpDocks, r => new RepairDock(r), (rpData, rpRawData) => rpData.Update(rpRawData)))
                OnPropertyChanged(nameof(RepairDocks));
        }

        internal void UpdateUnequippedEquipment() => UnequippedEquipmentUpdated(UnequippedEquipment);

        #endregion

        void ProcessUnequippedEquipment(JToken rpJson)
        {
            foreach (var rEquipmentType in rpJson.Select(r => (JProperty)r))
            {
                var rMatch = r_UnequippedEquipmentRegex.Match(rEquipmentType.Name);
                if (!rMatch.Success)
                    continue;

                var rType = (EquipmentType)int.Parse(rMatch.Value);
                if (rEquipmentType.Value.Type != JTokenType.Array)
                    UnequippedEquipment[rType] = null;
                else
                    UnequippedEquipment[rType] = rEquipmentType.Value.Select(r =>
                    {
                        var rID = (int)r;

                        Equipment rEquipment;
                        if (!Equipment.TryGetValue(rID, out rEquipment))
                            AddEquipment(rEquipment = new Equipment(new RawEquipment() { ID = rID, EquipmentID = -1 }));

                        return rEquipment;
                    }).ToArray();
            }

            UpdateUnequippedEquipment();
        }
        void RemoveEquipmentFromUnequippedList(Equipment rpEquipment)
        {
            UnequippedEquipment[rpEquipment.Info.Type] = UnequippedEquipment[rpEquipment.Info.Type].Where(r => r != rpEquipment).ToArray();
            UpdateUnequippedEquipment();
        }
        void RemoveEquipmentFromUnequippedList(IEnumerable<Equipment> rpEquipment)
        {
            foreach (var rEquipment in rpEquipment)
                UnequippedEquipment[rEquipment.Info.Type] = UnequippedEquipment[rEquipment.Info.Type].Where(r => r != rEquipment).ToArray();
            UpdateUnequippedEquipment();
        }
    }
}
