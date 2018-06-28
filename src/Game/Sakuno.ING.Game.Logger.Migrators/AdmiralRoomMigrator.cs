using System;
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
                using (var reader = scFile.OpenText())
                {
                    await reader.ReadLineAsync();
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (!string.IsNullOrWhiteSpace(line))
                            continue;
                        var s = line.Split(',');
                        if (s.Length < 11) continue;
                        DateTimeOffset time = DateTime.SpecifyKind(DateTime.Parse(s[0]) - timeZoneOffset, DateTimeKind.Utc);
                        if (range?.Contains(time) == false) continue;

                        if (context.ShipCreationTable.Find(time) != null)
                            context.ShipCreationTable.Add(new ShipCreation
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
                }

            if (selectedTypes.HasFlag(LogType.EquipmentCreation) &&
                folder.TryGetFile("createitem.csv", out var ecFile))
                using (var reader = ecFile.OpenText())
                {
                    await reader.ReadLineAsync();
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (!string.IsNullOrWhiteSpace(line))
                            continue;
                        var s = line.Split(',');
                        if (s.Length < 10) continue;
                        DateTimeOffset time = DateTime.SpecifyKind(DateTime.Parse(s[0]) - timeZoneOffset, DateTimeKind.Utc);
                        if (range?.Contains(time) == false) continue;

                        if (context.EquipmentCreationTable.Find(time) != null)
                            context.EquipmentCreationTable.Add(new EquipmentCreation
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

            if (selectedTypes.HasFlag(LogType.ExpeditionCompletion) &&
                folder.TryGetFile("mission.csv", out var exFile))
            {
                var expeditionTable = Compositor.Static<NavalBase>().MasterData.Expeditions;
                using (var reader = exFile.OpenText())
                {
                    await reader.ReadLineAsync();
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (!string.IsNullOrWhiteSpace(line))
                            continue;
                        var s = line.Split(',');
                        if (s.Length < 11) continue;
                        DateTimeOffset time = DateTime.SpecifyKind(DateTime.Parse(s[0]) - timeZoneOffset, DateTimeKind.Utc);
                        if (range?.Contains(time) == false) continue;

                        if (context.ExpeditionCompletionTable.Find(time) != null)
                            context.ExpeditionCompletionTable.Add(new ExpeditionCompletion
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
            }
        }
    }
}
