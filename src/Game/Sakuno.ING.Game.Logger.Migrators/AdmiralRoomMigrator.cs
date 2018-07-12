using System;
using System.Collections.Generic;
using System.Linq;
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

        ValueTask<IReadOnlyCollection<ShipCreationEntity>> ILogProvider<ShipCreationEntity>.GetLogsAsync(IFileSystemFacade source, TimeSpan timeZone)
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

        ValueTask<IReadOnlyCollection<EquipmentCreationEntity>> ILogProvider<EquipmentCreationEntity>.GetLogsAsync(IFileSystemFacade source, TimeSpan timeZone)
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

        ValueTask<IReadOnlyCollection<ExpeditionCompletionEntity>> ILogProvider<ExpeditionCompletionEntity>.GetLogsAsync(IFileSystemFacade source, TimeSpan timeZone)
        {
            var expeditionTable = Compositor.Static<NavalBase>().MasterData.Expeditions;

            return Helper.ParseCsv(source, "mission.csv", 11,
                s => new ExpeditionCompletionEntity
                {
                    TimeStamp = DateTime.SpecifyKind(DateTime.Parse(s[0]), DateTimeKind.Utc),
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
}
