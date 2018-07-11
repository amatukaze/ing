using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.IO;

namespace Sakuno.ING.Game.Logger.Migrators
{
    [Export(typeof(ILogMigrator))]
    internal class AdmiralRoomMigrator : ILogMigrator,
        ILogProvider<ShipCreationEntity>,
        ILogProvider<EquipmentCreationEntity>,
        ILogProvider<ExpeditionCompletionEntity>
    {
        public bool RequireFolder => true;
        public string Id => "AdmiralRoom";
        public string Title => "提督の部屋";

        async ValueTask<IReadOnlyCollection<ShipCreationEntity>> ILogProvider<ShipCreationEntity>.GetLogsAsync(IFileSystemFacade source, TimeSpan timeZone)
        {
            var folder = source as IFolderFacade ?? throw new ArgumentException("Source must be a folder.");
            var file = await folder.GetFileAsync("createship.csv");
            if (file == null) return Array.Empty<ShipCreationEntity>();

            var table = new List<ShipCreationEntity>();
            using (var reader = new StreamReader(await file.OpenReadAsync(), Encoding.UTF8))
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

                    table.Add(new ShipCreationEntity
                    {
                        TimeStamp = time,
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
                }
            }
            return table;
        }

        async ValueTask<IReadOnlyCollection<EquipmentCreationEntity>> ILogProvider<EquipmentCreationEntity>.GetLogsAsync(IFileSystemFacade source, TimeSpan timeZone)
        {
            var folder = source as IFolderFacade ?? throw new ArgumentException("Source must be a folder.");
            var file = await folder.GetFileAsync("createitem.csv");
            if (file == null) return Array.Empty<EquipmentCreationEntity>();

            var table = new List<EquipmentCreationEntity>();
            using (var reader = new StreamReader(await file.OpenReadAsync(), Encoding.UTF8))
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

                    table.Add(new EquipmentCreationEntity
                    {
                        TimeStamp = time,
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
                }
            }
            return table;
        }

        async ValueTask<IReadOnlyCollection<ExpeditionCompletionEntity>> ILogProvider<ExpeditionCompletionEntity>.GetLogsAsync(IFileSystemFacade source, TimeSpan timeZone)
        {
            var folder = source as IFolderFacade ?? throw new ArgumentException("Source must be a folder.");
            var file = await folder.GetFileAsync("mission.csv");
            if (file == null) return Array.Empty<ExpeditionCompletionEntity>();

            var expeditionTable = Compositor.Static<NavalBase>().MasterData.Expeditions;
            var table = new List<ExpeditionCompletionEntity>();
            using (var reader = new StreamReader(await file.OpenReadAsync(), Encoding.UTF8))
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

                    table.Add(new ExpeditionCompletionEntity
                    {
                        TimeStamp = time,
                        ExpeditionName = s[1],
                        ExpeditionId = expeditionTable.FirstOrDefault(e => e.Name == s[1])?.Id ?? default,
                        Result = (ExpeditionResult)int.Parse(s[2]),
                        MaterialsAcquired = new Materials
                        {
                            Fuel = int.Parse(s[3]),
                            Bullet = int.Parse(s[4]),
                            Steel = int.Parse(s[5]),
                            Bauxite = int.Parse(s[6])
                        },
                        RewardItem1 = int.Parse(s[7]) > 0 ?
                            new ItemRecord
                            {
                                ItemId = (UseItemId)int.Parse(s[7]),
                                Count = int.Parse(s[8])
                            } : (ItemRecord?)null,
                        RewardItem2 = int.Parse(s[9]) > 0 ?
                            new ItemRecord
                            {
                                ItemId = (UseItemId)int.Parse(s[9]),
                                Count = int.Parse(s[10])
                            } : (ItemRecord?)null
                    });
                }
            }
            return table;
        }
    }
}
