using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger.Migrators
{
    [Export(typeof(ILogMigrator))]
    internal class AdmiralRoomMigrator : ILogMigrator
    {
        public LogType SupportedTypes
            => LogType.ShipCreation
            | LogType.EquipmentCreation
            | LogType.ExpeditionCompletion;
        public bool RequireFolder => true;
        public string Id => "提督の部屋";

        public async ValueTask MigrateAsync(FileSystemInfo source, LoggerContext context, LogType selectedTypes, TimeSpan timeZoneOffset, TimeRange? range)
        {
            var folder = source as DirectoryInfo ?? throw new ArgumentException("Source must be a folder.");

            if (selectedTypes.HasFlag(LogType.ShipCreation) &&
                folder.TryGetFile("createship.csv", out var scFile))
            {
                var index = new HashSet<DateTimeOffset>(context.ShipCreationTable.Select(x => x.TimeStamp));
                var table = new List<ShipCreation>();
                using (var reader = scFile.OpenText())
                {
                    await reader.ReadLineAsync();
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (string.IsNullOrWhiteSpace(line))
                            continue;
                        var s = line.Split(',');
                        if (s.Length < 12) continue;
                        DateTimeOffset time = DateTime.SpecifyKind(DateTime.Parse(s[0]) - timeZoneOffset, DateTimeKind.Utc);
                        if (range?.Contains(time) == false) continue;
                        if (index.Contains(time)) continue;

                        table.Add(new ShipCreation
                        {
                            TimeStamp = time,
                            SecretaryLevel = int.Parse(s[1]),
                            Secretary = (ShipInfoId)int.Parse(s[2]),
                            Consumption = new MaterialsEntity
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
                    }
                    context.ShipCreationTable.AddRange(table);
                }
            }

            if (selectedTypes.HasFlag(LogType.EquipmentCreation) &&
                folder.TryGetFile("createitem.csv", out var ecFile))
            {
                var index = new HashSet<DateTimeOffset>(context.EquipmentCreationTable.Select(x => x.TimeStamp));
                var table = new List<EquipmentCreation>();
                using (var reader = ecFile.OpenText())
                {
                    await reader.ReadLineAsync();
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (string.IsNullOrWhiteSpace(line))
                            continue;
                        var s = line.Split(',');
                        if (s.Length < 10) continue;
                        DateTimeOffset time = DateTime.SpecifyKind(DateTime.Parse(s[0]) - timeZoneOffset, DateTimeKind.Utc);
                        if (range?.Contains(time) == false) continue;
                        if (index.Contains(time)) continue;

                        table.Add(new EquipmentCreation
                        {
                            TimeStamp = time,
                            SecretaryLevel = int.Parse(s[1]),
                            Secretary = (ShipInfoId)int.Parse(s[2]),
                            Consumption = new MaterialsEntity
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
                    }
                }
                context.EquipmentCreationTable.AddRange(table);
            }

            if (selectedTypes.HasFlag(LogType.ExpeditionCompletion) &&
                folder.TryGetFile("mission.csv", out var exFile))
            {
                var expeditionTable = Compositor.Static<NavalBase>().MasterData.Expeditions;
                var index = new HashSet<DateTimeOffset>(context.ExpeditionCompletionTable.Select(x => x.TimeStamp));
                var table = new List<ExpeditionCompletion>();
                using (var reader = exFile.OpenText())
                {
                    await reader.ReadLineAsync();
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (string.IsNullOrWhiteSpace(line))
                            continue;
                        var s = line.Split(',');
                        if (s.Length < 11) continue;
                        DateTimeOffset time = DateTime.SpecifyKind(DateTime.Parse(s[0]) - timeZoneOffset, DateTimeKind.Utc);
                        if (range?.Contains(time) == false) continue;
                        if (index.Contains(time)) continue;

                        table.Add(new ExpeditionCompletion
                        {
                            TimeStamp = time,
                            ExpeditionName = s[1],
                            ExpeditionId = expeditionTable.FirstOrDefault(e => e.Name == s[1])?.Id ?? default,
                            Result = (ExpeditionResult)int.Parse(s[2]),
                            MaterialsAcquired = new MaterialsEntity
                            {
                                Fuel = int.Parse(s[3]),
                                Bullet = int.Parse(s[4]),
                                Steel = int.Parse(s[5]),
                                Bauxite = int.Parse(s[6])
                            },
                            RewardItem1 = new ItemRecordEntity
                            {
                                ItemId = (UseItemId)int.Parse(s[7]),
                                Count = int.Parse(s[8])
                            },
                            RewardItem2 = new ItemRecordEntity
                            {
                                ItemId = (UseItemId)int.Parse(s[9]),
                                Count = int.Parse(s[10])
                            }
                        });
                    }
                }
                context.ExpeditionCompletionTable.AddRange(table);
            }
        }
    }
}
