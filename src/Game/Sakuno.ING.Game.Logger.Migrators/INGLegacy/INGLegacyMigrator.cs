using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger.Migrators.INGLegacy
{
    [Export(typeof(ILogMigrator))]
    internal class INGLegacyMigrator : ILogMigrator,
        ILogProvider<ShipCreation>,
        ILogProvider<EquipmentCreation>,
        ILogProvider<ExpeditionCompletion>
    {
        public bool RequireFolder => false;
        public string Id => "Intelligent Naval Gun (Old)";

        async ValueTask<IReadOnlyCollection<ShipCreation>> ILogProvider<ShipCreation>.GetLogsAsync(FileSystemInfo source, TimeSpan timeZone)
        {
            using (var context = new INGLegacyContext(source.FullName))
                return (await context.ConstructionTable.ToListAsync())
                    .Select(x => new ShipCreation
                    {
                        TimeStamp = DateTimeOffset.FromUnixTimeSeconds(x.time),
                        ShipBuilt = (ShipInfoId)x.ship,
                        Consumption = new MaterialsEntity
                        {
                            Fuel = x.fuel,
                            Bullet = x.bullet,
                            Steel = x.steel,
                            Bauxite = x.bauxite,
                            Development = x.dev_material
                        },
                        Secretary = (ShipInfoId)x.flagship,
                        AdmiralLevel = x.hq_level,
                        IsLSC = x.is_lsc,
                        EmptyDockCount = (x.empty_dock ?? -1) + 1
                    }).ToList();
        }

        async ValueTask<IReadOnlyCollection<EquipmentCreation>> ILogProvider<EquipmentCreation>.GetLogsAsync(FileSystemInfo source, TimeSpan timeZone)
        {
            using (var context = new INGLegacyContext(source.FullName))
                return (await context.DevelopmentTable.ToListAsync())
                    .Select(x => new EquipmentCreation
                    {
                        TimeStamp = DateTimeOffset.FromUnixTimeSeconds(x.time),
                        IsSuccess = x.equipment != null,
                        EquipmentCreated = (EquipmentInfoId)(x.equipment ?? 0),
                        Consumption = new MaterialsEntity
                        {
                            Fuel = x.fuel,
                            Bullet = x.bullet,
                            Steel = x.steel,
                            Bauxite = x.bauxite
                        },
                        Secretary = (ShipInfoId)x.flagship,
                        AdmiralLevel = x.hq_level
                    }).ToList();
        }

        async ValueTask<IReadOnlyCollection<ExpeditionCompletion>> ILogProvider<ExpeditionCompletion>.GetLogsAsync(FileSystemInfo source, TimeSpan timeZone)
        {
            var expeditions = Compositor.Static<NavalBase>().MasterData.Expeditions;
            using (var context = new INGLegacyContext(source.FullName))
                return (await context.ExpeditionTable.ToListAsync())
                    .Select(x => new ExpeditionCompletion
                    {
                        TimeStamp = DateTimeOffset.FromUnixTimeSeconds(x.time),
                        ExpeditionId = (ExpeditionId)x.expedition,
                        ExpeditionName = expeditions[(ExpeditionId)x.expedition].Name,
                        Result = (ExpeditionResult)x.result,
                        MaterialsAcquired = new MaterialsEntity
                        {
                            Fuel = x.fuel ?? 0,
                            Bullet = x.bullet ?? 0,
                            Steel = x.steel ?? 0,
                            Bauxite = x.bauxite ?? 0
                        },
                        RewardItem1 = new ItemRecordEntity
                        {
                            ItemId = (UseItemId)(x.item1 ?? 0),
                            Count = x.item1_count ?? 0
                        },
                        RewardItem2 = new ItemRecordEntity
                        {
                            ItemId = (UseItemId)(x.item2 ?? 0),
                            Count = x.item2_count ?? 0
                        }
                    }).ToList();
        }
    }
}
