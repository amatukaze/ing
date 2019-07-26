using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.IO;

namespace Sakuno.ING.Game.Logger.Migrators.INGLegacy
{
    [Export(typeof(LogMigrator))]
    public class INGLegacyMigrator : LogMigrator
    {
        public override bool RequireFolder => false;
        public override string Id => "ING Legacy";
        public override string Title => "Intelligent Naval Gun (Old)";

        public override bool SupportShipCreation => true;
        public override async ValueTask<IReadOnlyCollection<ShipCreationEntity>> GetShipCreationAsync(IFileSystemFacade source, TimeSpan timeZone)
        {
            using var context = new INGLegacyContext(await (source as IFileFacade ?? throw new ArgumentException("Source must be a file.")).GetAccessPathAsync());
            return (await context.ConstructionTable.ToListAsync())
                .Select(x => new ShipCreationEntity
                {
                    TimeStamp = DateTimeOffset.FromUnixTimeSeconds(x.time),
                    ShipBuilt = (ShipInfoId)x.ship,
                    Consumption = new Materials
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
                    EmptyDockCount = x.empty_dock ?? 0
                }).ToList();
        }

        public override bool SupportEquipmentCreation => true;
        public override async ValueTask<IReadOnlyCollection<EquipmentCreationEntity>> GetEquipmentCreationAsync(IFileSystemFacade source, TimeSpan timeZone)
        {
            using var context = new INGLegacyContext(await (source as IFileFacade ?? throw new ArgumentException("Source must be a file.")).GetAccessPathAsync());
            return (await context.DevelopmentTable.ToListAsync())
                .Select(x => new EquipmentCreationEntity
                {
                    TimeStamp = DateTimeOffset.FromUnixTimeSeconds(x.time),
                    IsSuccess = x.equipment != null,
                    EquipmentCreated = (EquipmentInfoId)(x.equipment ?? 0),
                    Consumption = new Materials
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

        public override bool SupportExpeditionCompletion => true;
        public override async ValueTask<IReadOnlyCollection<ExpeditionCompletionEntity>> GetExpeditionCompletionAsync(IFileSystemFacade source, TimeSpan timeZone)
        {
            var expeditions = Compositor.Static<NavalBase>().MasterData.Expeditions;
            using var context = new INGLegacyContext(await (source as IFileFacade ?? throw new ArgumentException("Source must be a file.")).GetAccessPathAsync());
            return (await context.ExpeditionTable.ToListAsync())
                .Select(x => new ExpeditionCompletionEntity
                {
                    TimeStamp = DateTimeOffset.FromUnixTimeSeconds(x.time),
                    ExpeditionId = (ExpeditionId)x.expedition,
                    ExpeditionName = expeditions[(ExpeditionId)x.expedition].Name.Origin,
                    Result = (ExpeditionResult)x.result,
                    MaterialsAcquired = new Materials
                    {
                        Fuel = x.fuel ?? 0,
                        Bullet = x.bullet ?? 0,
                        Steel = x.steel ?? 0,
                        Bauxite = x.bauxite ?? 0
                    },
                    RewardItem1_ItemId = x.item1,
                    RewardItem1_Count = x.item1_count,
                    RewardItem2_ItemId = x.item2,
                    RewardItem2_Count = x.item2_count
                }).ToList();
        }
    }
}
