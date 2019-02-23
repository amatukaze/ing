using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Logger.Entities.Combat;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.IO;

namespace Sakuno.ING.Game.Logger.Migrators
{
    [Export(typeof(LogMigrator))]
    internal class AdmiralRoomMigrator : LogMigrator
    {
        public override bool RequireFolder => true;
        public override string Id => "AdmiralRoom";
        public override string Title => "提督の部屋";

        public override bool SupportShipCreation => true;
        public override ValueTask<IReadOnlyCollection<ShipCreationEntity>> GetShipCreationAsync(IFileSystemFacade source, TimeSpan timeZone)
            => Helper.ParseCsv(source, "createship.csv", 12,
                s => new ShipCreationEntity
                {
                    TimeStamp = DateTime.SpecifyKind(DateTime.Parse(s[0]), DateTimeKind.Utc),
                    SecretaryLevel = int.Parse(s[1]),
                    Secretary = (ShipInfoId)int.Parse(s[2]),
                    Consumption = new Materials
                    {
                        Fuel = int.Parse(s[3]),
                        Bullet = int.Parse(s[4]),
                        Steel = int.Parse(s[5]),
                        Bauxite = int.Parse(s[6]),
                        Development = int.Parse(s[7])
                    },
                    IsLSC = bool.Parse(s[8]),
                    ShipBuilt = (ShipInfoId)int.Parse(s[9]),
                    EmptyDockCount = int.Parse(s[10]),
                    AdmiralLevel = int.Parse(s[11])
                });

        public override bool SupportEquipmentCreation => true;
        public override ValueTask<IReadOnlyCollection<EquipmentCreationEntity>> GetEquipmentCreationAsync(IFileSystemFacade source, TimeSpan timeZone)
            => Helper.ParseCsv(source, "createitem.csv", 10,
                s => new EquipmentCreationEntity
                {
                    TimeStamp = DateTime.SpecifyKind(DateTime.Parse(s[0]), DateTimeKind.Utc),
                    SecretaryLevel = int.Parse(s[1]),
                    Secretary = (ShipInfoId)int.Parse(s[2]),
                    Consumption = new Materials
                    {
                        Fuel = int.Parse(s[3]),
                        Bullet = int.Parse(s[4]),
                        Steel = int.Parse(s[5]),
                        Bauxite = int.Parse(s[6])
                    },
                    IsSuccess = bool.Parse(s[7]),
                    EquipmentCreated = (EquipmentInfoId)int.Parse(s[8]),
                    AdmiralLevel = int.Parse(s[9])
                });

        public override bool SupportExpeditionCompletion => true;
        public override ValueTask<IReadOnlyCollection<ExpeditionCompletionEntity>> GetExpeditionCompletionAsync(IFileSystemFacade source, TimeSpan timeZone)
        {
            var expeditionTable = Compositor.Static<NavalBase>().MasterData.Expeditions;

            return Helper.ParseCsv(source, "mission.csv", 11,
                s => new ExpeditionCompletionEntity
                {
                    TimeStamp = DateTime.SpecifyKind(DateTime.Parse(s[0]), DateTimeKind.Utc),
                    ExpeditionName = s[1],
                    ExpeditionId = expeditionTable.FirstOrDefault(e => e.Name.Origin == s[1])?.Id ?? default,
                    Result = (ExpeditionResult)int.Parse(s[2]),
                    MaterialsAcquired = new Materials
                    {
                        Fuel = int.Parse(s[3]),
                        Bullet = int.Parse(s[4]),
                        Steel = int.Parse(s[5]),
                        Bauxite = int.Parse(s[6])
                    },
                    RewardItem1 = int.Parse(s[7]) > 0 ?
                            new UseItemRecord
                            {
                                ItemId = (UseItemId)int.Parse(s[7]),
                                Count = int.Parse(s[8])
                            } : (UseItemRecord?)null,
                    RewardItem2 = int.Parse(s[9]) > 0 ?
                            new UseItemRecord
                            {
                                ItemId = (UseItemId)int.Parse(s[9]),
                                Count = int.Parse(s[10])
                            } : (UseItemRecord?)null
                });
        }

        public override bool SupportBattleAndDrop => true;
        public override async ValueTask<IReadOnlyCollection<BattleEntity>> GetBattleAndDropAsync(IFileSystemFacade source, TimeSpan timeZone)
        {
            var results = (await Helper.ParseCsv(source, "drop.csv", 9,
                s => new BattleEntity
                {
                    TimeStamp = DateTime.SpecifyKind(DateTime.Parse(s[0]), DateTimeKind.Utc),
                    CompletionTime = DateTime.SpecifyKind(DateTime.Parse(s[0]), DateTimeKind.Utc),
                    MapId = (MapId)int.Parse(s[1]),
                    MapName = s[2],
                    RouteId = int.Parse(s[3]),
                    EventKind = bool.Parse(s[4]) ? MapEventKind.Boss : MapEventKind.Battle,
                    Rank = SwitchRank(s[5]),
                    EnemyFleetName = s[6],
                    ShipDropped = int.Parse(s[7]) > 0 ? (ShipInfoId)int.Parse(s[7]) : (ShipInfoId?)null,
                    UseItemAcquired = int.Parse(s[8]) > 0 ? (UseItemId)int.Parse(s[8]) : (UseItemId?)null
                })).ToDictionary(x => x.TimeStamp);

            BattleRank SwitchRank(string text)
            {
                switch (text)
                {
                    case "Perfect":
                        return BattleRank.Perfect;
                    case "S":
                        return BattleRank.S;
                    case "A":
                        return BattleRank.A;
                    case "B":
                        return BattleRank.B;
                    case "C":
                        return BattleRank.C;
                    case "D":
                        return BattleRank.D;
                    case "E":
                        return BattleRank.E;
                    default:
                        return default;
                }
            }

            var folder = source as IFolderFacade ?? throw new ArgumentException("Source must be a folder.");
            var details = await folder.GetFolderAsync("battlelog");
            if (details != null)
                foreach (var file in await details.GetFilesAsync())
                {
                    switch (Path.GetExtension(file.FullName))
                    {
                        case ".zip":
                            using (var zip = new ZipArchive(await file.OpenReadAsync(), ZipArchiveMode.Read, false))
                                foreach (var entry in zip.Entries)
                                    if (entry.Name.EndsWith(".log"))
                                        using (var stream = entry.Open())
                                            await ImportDetailAsync(stream);
                            break;
                        case ".log":
                            using (var stream = await file.OpenReadAsync())
                                await ImportDetailAsync(stream);
                            break;
                    }
                }

            return results.Values;

            async ValueTask ImportDetailAsync(Stream stream)
            {
                var reader = new StreamReader(stream);
                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();
                    if (line.IndexOf("\"battle\"") != line.LastIndexOf("\"battle\""))
                        line = line.Insert(line.LastIndexOf("\"battle\"") + 1, "night");
                    var log = JsonConvert.DeserializeObject<BattleDetailLog>(line);
                    DateTimeOffset t = DateTime.SpecifyKind(DateTime.Parse(log.time), DateTimeKind.Utc);
                    if (results.TryGetValue(t, out var e))
                    {
                        e.Details = new BattleDetailEntity();
                        e.BattleKind = log.startnext.data.api_data.api_event_kind;
                        if (log.startnext?.data?.api_data?.api_destruction_battle != null)
                            e.Details.LandBaseDefence = log.startnext.data.api_data.api_destruction_battle.ToString(Formatting.None);
                        switch (log.battle?.api)
                        {
                            case "api_req_sortie/battle":
                            case "api_req_battle_midnight/sp_midnight":
                            case "api_req_sortie/airbattle":
                            case "api_req_sortie/ld_airbattle":
                            case "api_req_sortie/ld_shooting":
                            case "api_req_combined_battle/ec_battle":
                                e.CombinedFleetType = CombinedFleetType.None;
                                break;
                            case "api_req_combined_battle/airbattle":
                            case "api_req_combined_battle/battle":
                            case "api_req_combined_battle/sp_midnight":
                            case "api_req_combined_battle/each_battle":
                            case "api_req_combined_battle/ld_airbattle":
                            case "api_req_combined_battle/ld_shooting":
                                e.CombinedFleetType = CombinedFleetType.CarrierTaskForceFleet;
                                break;
                            case "api_req_combined_battle/battle_water":
                            case "api_req_combined_battle/each_battle_water":
                                e.CombinedFleetType = CombinedFleetType.SurfaceTaskForceFleet;
                                break;
                        }
                        if (log.battle?.data.api_data != null)
                            e.Details.FirstBattleDetail = log.battle.data.api_data.ToString(Formatting.None);
                        if (log.nightbattle?.data.api_data != null)
                            e.Details.SecondBattleDetail = log.nightbattle.data.api_data.ToString(Formatting.None);
                        e.Details.SortieFleetState = SelectFleet(log.fleet1);
                        e.Details.SortieFleet2State = SelectFleet(log.fleet2);
                    }
                }

                ShipInBattleEntity[] SelectFleet(BattleDetailLog.Ship[] fleet)
                    => fleet?.Select(x => new ShipInBattleEntity
                    {
                        Id = (ShipInfoId)x.shipid,
                        Level = x.lv,
                        Firepower = new ShipMordenizationStatus(x.karyoku),
                        Torpedo = new ShipMordenizationStatus(x.raisou),
                        AntiAir = new ShipMordenizationStatus(x.taiku),
                        Armor = new ShipMordenizationStatus(x.soukou),
                        Evasion = new ShipMordenizationStatus(x.kaihi),
                        AntiSubmarine = new ShipMordenizationStatus(x.taisen),
                        LineOfSight = new ShipMordenizationStatus(x.sakuteki),
                        Luck = new ShipMordenizationStatus(x.lucky),
                        Slots = x.slots.Select(SelectSlot).ToArray(),
                        ExtraSlot = SelectSlot(x.slotex)
                    }).ToArray();

                SlotInBattleEntity SelectSlot(BattleDetailLog.Equipment slot)
                {
                    if (slot is null) return default;
                    return new SlotInBattleEntity
                    {
                        Id = (EquipmentInfoId)slot.itemid,
                        AirProficiency = slot.alv,
                        ImprovementLevel = slot.level
                    };
                }
            }

            #region System.Text.Json version
            //async ValueTask ImportDetailAsync(Stream stream, Dictionary<DateTimeOffset, BattleEntity> r)
            //{
            //    byte[] data = new byte[stream.Length];
            //    await stream.ReadAsync(data, 0, data.Length);
            //    ImportDetailCore(data, r);
            //}

            //void ImportDetailCore(byte[] data, Dictionary<DateTimeOffset, BattleEntity> r)
            //{
            //    ReadOnlyMemory<byte> remained = data;
            //    ReadOnlySpan<byte> newLine = stackalloc byte[] { (byte)'\r', (byte)'\n' };
            //    while (true)
            //    {
            //        int i = remained.Span.IndexOf(newLine);
            //        if (i <= 0) return;
            //        var line = remained.Slice(0, i);
            //        remained = remained.Slice(i + newLine.Length);
            //        var document = JsonDocument.Parse(line);
            //        DateTimeOffset t = DateTime.SpecifyKind(DateTime.Parse(document.RootElement.GetProperty("time").GetString()), DateTimeKind.Utc);
            //        if (r.TryGetValue(t, out var e))
            //        {
            //            e.UnstoredDetails = new UnstoredBattleDetail();
            //            foreach (var p in document.RootElement.EnumerateObject())
            //                switch (p.Name) // for potential duplicated property
            //                {
            //                    case "startnext":
            //                        var startnext = p.Value.GetProperty("data").GetProperty("api_data");
            //                        e.BattleKind = (BattleKind)startnext.GetProperty("api_event_kind").GetInt32();
            //                        if (startnext.TryGetProperty("api_destruction_battle", out var defence))
            //                            e.UnstoredDetails.LandBaseDefence = defence;
            //                        break;
            //                    case "battle":
            //                        if (e.UnstoredDetails.FirstBattleDetail != null)
            //                        {
            //                            switch (p.Value.GetProperty("api").GetString())
            //                            {
            //                                case "api_req_sortie/battle":
            //                                case "api_req_battle_midnight/sp_midnight":
            //                                case "api_req_sortie/airbattle":
            //                                case "api_req_sortie/ld_airbattle":
            //                                case "api_req_sortie/ld_shooting":
            //                                case "api_req_combined_battle/ec_battle":
            //                                    e.CombinedFleetType = CombinedFleetType.None;
            //                                    break;
            //                                case "api_req_combined_battle/airbattle":
            //                                case "api_req_combined_battle/battle":
            //                                case "api_req_combined_battle/sp_midnight":
            //                                case "api_req_combined_battle/each_battle":
            //                                case "api_req_combined_battle/ld_airbattle":
            //                                case "api_req_combined_battle/ld_shooting":
            //                                    e.CombinedFleetType = CombinedFleetType.CarrierTaskForceFleet;
            //                                    break;
            //                                case "api_req_combined_battle/battle_water":
            //                                case "api_req_combined_battle/each_battle_water":
            //                                    e.CombinedFleetType = CombinedFleetType.SurfaceTaskForceFleet;
            //                                    break;
            //                            }
            //                            e.UnstoredDetails.FirstBattleDetail = p.Value.GetProperty("data").GetProperty("api_data");
            //                        }
            //                        else goto case "nightbattle";
            //                        break;
            //                    case "nightbattle":
            //                        e.UnstoredDetails.SecondBattleDetail = p.Value.GetProperty("data").GetProperty("api_data");
            //                        break;
            //                    case "fleet1":

            //                        break;
            //                }

            //        }
            //    }
            //}
            #endregion
        }

        private class BattleDetailLog
        {
            public string time;
            public class ApiLog<T>
            {
                public string api;
                public class SvData
                {
                    public T api_data;
                }
                public SvData data;
            }
            public class StartNext
            {
                public BattleKind api_event_kind;
                public JObject api_destruction_battle;
            }
            public ApiLog<StartNext> startnext;
            public ApiLog<JObject> battle;
            public ApiLog<JObject> nightbattle;
            public class Equipment
            {
                public int itemid;
                public int level;
                public int alv;
            }
            public class Ship
            {
                public int id;
                public int shipid;
                public int lv;
                public int karyoku;
                public int raisou;
                public int taiku;
                public int soukou;
                public int kaihi;
                public int taisen;
                public int sakuteki;
                public int lucky;
                public Equipment[] slots;
                public Equipment slotex;
            }
            public Ship[] fleet1;
            public Ship[] fleet2;
        }
    }
}
