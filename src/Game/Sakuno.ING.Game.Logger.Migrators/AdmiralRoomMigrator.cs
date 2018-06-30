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
    internal class AdmiralRoomMigrator : ILogMigrator,
        ILogProvider<ShipCreation>,
        ILogProvider<EquipmentCreation>,
        ILogProvider<ExpeditionCompletion>
    {
        public bool RequireFolder => true;
        public string Id => "提督の部屋";

        async ValueTask<IReadOnlyCollection<ShipCreation>> ILogProvider<ShipCreation>.GetLogsAsync(FileSystemInfo source, TimeSpan timeZone)
        {
            var folder = source as DirectoryInfo ?? throw new ArgumentException("Source must be a folder.");
            if (!folder.TryGetFile("createship.csv", out var file)) return Array.Empty<ShipCreation>();

            var table = new List<ShipCreation>();
            using (var reader = file.OpenText())
            {
                await reader.ReadLineAsync();
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    var s = line.Split(',');
                    if (s.Length < 12) continue;
                    DateTimeOffset time = DateTime.SpecifyKind(DateTime.Parse(s[0]) - timeZone, DateTimeKind.Utc);

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
            }
            return table;
        }

        async ValueTask<IReadOnlyCollection<EquipmentCreation>> ILogProvider<EquipmentCreation>.GetLogsAsync(FileSystemInfo source, TimeSpan timeZone)
        {
            var folder = source as DirectoryInfo ?? throw new ArgumentException("Source must be a folder.");
            if (!folder.TryGetFile("createitem.csv", out var scFile)) return Array.Empty<EquipmentCreation>();

            var table = new List<EquipmentCreation>();
            using (var reader = scFile.OpenText())
            {
                await reader.ReadLineAsync();
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    var s = line.Split(',');
                    if (s.Length < 10) continue;
                    DateTimeOffset time = DateTime.SpecifyKind(DateTime.Parse(s[0]) - timeZone, DateTimeKind.Utc);

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
            return table;
        }

        async ValueTask<IReadOnlyCollection<ExpeditionCompletion>> ILogProvider<ExpeditionCompletion>.GetLogsAsync(FileSystemInfo source, TimeSpan timeZone)
        {
            var folder = source as DirectoryInfo ?? throw new ArgumentException("Source must be a folder.");
            if (!folder.TryGetFile("mission.csv", out var scFile)) return Array.Empty<ExpeditionCompletion>();

            var expeditionTable = Compositor.Static<NavalBase>().MasterData.Expeditions;
            var table = new List<ExpeditionCompletion>();
            using (var reader = scFile.OpenText())
            {
                await reader.ReadLineAsync();
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    var s = line.Split(',');
                    if (s.Length < 11) continue;
                    DateTimeOffset time = DateTime.SpecifyKind(DateTime.Parse(s[0]) - timeZone, DateTimeKind.Utc);

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
            return table;
        }
    }
}
