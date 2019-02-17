using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sakuno.ING.Composition;
using Sakuno.ING.Data;
using Sakuno.ING.Game.Logger.BinaryJson;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Logger.Entities.Combat;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger
{
    [Export(typeof(Logger), LazyCreate = false)]
    public class Logger
    {
        private readonly IDataService dataService;
        private readonly NavalBase navalBase;

        private ShipCreationEntity shipCreation;
        private BuildingDockId lastBuildingDock;

        private readonly object admiralLock = new object();
        private Admiral currentAdmiral;
        private BattleApiDeserializer battleApiDeserializer;

        private LoggerContext currentBattleContext;
        private Fleet currentFleetInBattle, currentFleet2InBattle;
        private CombinedFleetType currentCombinedFleet;
        private BattleEntity currentBattle;

        public Logger(IDataService dataService, GameProvider provider, NavalBase navalBase)
        {
            this.dataService = dataService;
            this.navalBase = navalBase;

            provider.EquipmentCreated += (t, m) =>
            {
                using (var context = CreateContext())
                {
                    context.EquipmentCreationTable.Add(new EquipmentCreationEntity
                    {
                        TimeStamp = t,
                        Consumption = m.Consumption,
                        EquipmentCreated = m.SelectedEquipentInfoId,
                        IsSuccess = m.IsSuccess,
                        AdmiralLevel = this.navalBase.Admiral.Leveling.Level,
                        Secretary = this.navalBase.Secretary.Info.Id,
                        SecretaryLevel = this.navalBase.Secretary.Leveling.Level
                    });
                    context.SaveChanges();
                }
            };

            provider.ShipCreated += (t, m) =>
            {
                shipCreation = new ShipCreationEntity
                {
                    TimeStamp = t,
                    Consumption = m.Consumption,
                    IsLSC = m.IsLSC,
                    AdmiralLevel = this.navalBase.Admiral.Leveling.Level,
                    Secretary = this.navalBase.Secretary.Info.Id,
                    SecretaryLevel = this.navalBase.Secretary.Leveling.Level
                };
                lastBuildingDock = m.BuildingDockId;
            };

            provider.BuildingDockUpdated += (t, m) =>
            {
                if (shipCreation != null)
                    using (var context = CreateContext())
                    {
                        shipCreation.ShipBuilt = m.Single(x => x.Id == lastBuildingDock).BuiltShipId.Value;
                        shipCreation.EmptyDockCount = this.navalBase.BuildingDocks.Count(x => x.State == BuildingDockState.Empty);
                        context.ShipCreationTable.Add(shipCreation);
                        shipCreation = null;
                        lastBuildingDock = default;
                        context.SaveChanges();
                    }
            };

            provider.ExpeditionCompleted += (t, m) =>
            {
                using (var context = CreateContext())
                {
                    context.ExpeditionCompletionTable.Add(new ExpeditionCompletionEntity
                    {
                        TimeStamp = t,
                        ExpeditionId = this.navalBase.Fleets[m.FleetId].Expedition.Id,
                        ExpeditionName = m.ExpeditionName,
                        Result = m.Result,
                        MaterialsAcquired = m.MaterialsAcquired,
                        RewardItem1 = m.RewardItem1,
                        RewardItem2 = m.RewardItem2
                    });
                    context.SaveChanges();
                }
            };

#if DEBUG
            InitializeAdmiral(null);
#endif

            navalBase.AdmiralChanging += (t, _, a) =>
            {
                if (a != null)
                    lock (admiralLock)
                        InitializeAdmiral(a);
            };

            provider.SortieStarting += (t, m) =>
            {
                currentFleetInBattle = this.navalBase.Fleets[m.FleetId];
                currentCombinedFleet = this.navalBase.CombinedFleet;
                if (currentCombinedFleet != CombinedFleetType.None)
                    currentFleet2InBattle = navalBase.Fleets[(FleetId)2];
                currentBattleContext = CreateContext();
            };

            provider.MapRouting += (t, m) =>
            {
                var map = this.navalBase.Maps[m.MapId];
                currentBattle = new BattleEntity
                {
                    TimeStamp = t,
                    MapId = m.MapId,
                    MapName = map.Info.Name.Origin,
                    RouteId = m.RouteId,
                    EventKind = m.EventKind,
                    BattleKind = m.BattleKind,
                    CombinedFleetType = this.navalBase.CombinedFleet,
                    MapRank = map.Rank,
                    MapGaugeType = map.GaugeType,
                    MapGaugeNumber = map.GaugeIndex,
                    MapGaugeHP = map.Gauge?.Current,
                    MapGaugeMaxHP = map.Gauge?.Max,
                    Details = new BattleDetailEntity()
                };
                currentBattleContext.BattleTable.Add(currentBattle);
                currentBattleContext.SaveChanges();
            };

            provider.BattleStarted += (t, m) =>
            {
                currentBattle.TimeStamp = t;
                currentBattle.Details.SortieFleetState = currentFleetInBattle.Ships.Select(x => new ShipInBattleEntity(x)).Store();
                currentBattle.Details.SortieFleet2State = currentFleet2InBattle?.Ships.Select(x => new ShipInBattleEntity(x)).Store();
                currentBattle.Details.FirstBattleDetail = currentBattleContext.StoreBattle(m.Unparsed, true);
                currentBattle.Details.LbasState = m.Parsed.LandBasePhases
                    .Select(x => new AirForceInBattle(this.navalBase.AirForce[(currentBattle.MapId.AreaId, x.GroupId)]))
                    .Store();
                currentBattleContext.ChangeTracker.DetectChanges();
                currentBattleContext.SaveChanges();
            };

            provider.BattleAppended += (t, m) =>
            {
                currentBattle.TimeStamp = t;
                currentBattle.Details.SecondBattleDetail = currentBattleContext.StoreBattle(m.Unparsed, true);
                currentBattleContext.ChangeTracker.DetectChanges();
                currentBattleContext.SaveChanges();
            };

            provider.BattleCompleted += (t, m) =>
            {
                currentBattle.TimeStamp = t;
                currentBattle.Rank = m.Rank;
                currentBattle.AdmiralExperience = m.AdmiralExperience;
                currentBattle.BaseExperience = m.BaseExperience;
                currentBattle.MapCleared = m.MapCleared;
                currentBattle.EnemyFleetName = m.EnemyFleetName;
                currentBattle.UseItemAcquired = m.UseItemAcquired;
                currentBattle.ShipDropped = m.ShipDropped;
                currentBattleContext.ChangeTracker.DetectChanges();
                currentBattleContext.SaveChanges();
            };

            provider.HomeportReturned += (t, m) =>
            {
                currentBattle = null;
                currentBattleContext?.Dispose();
                currentFleetInBattle = null;
                currentFleet2InBattle = null;
            };
        }

        private void InitializeAdmiral(Admiral admiral)
        {
            using (var context = new LoggerContextBase(ConfigureContext(admiral?.Id)))
            {
                context.Database.Migrate();
                battleApiDeserializer = new BattleApiDeserializer(new BinaryJsonIdResolver(context.JNameTable));
            }
            currentAdmiral = admiral;
        }

        public bool PlayerLoaded
#if DEBUG
            => true;
#else
            => navalBase.Admiral != null;
#endif

        public LoggerContext CreateContext()
        {
            lock (admiralLock)
                return new LoggerContext(ConfigureContext(currentAdmiral?.Id),
                    battleApiDeserializer ?? throw new InvalidOperationException("Game not loaded"));
        }

        private DbContextOptions<LoggerContextBase> ConfigureContext(int? admiralId)
            => dataService.ConfigureDbContext<LoggerContextBase>(admiralId?.ToString() ?? "0", "logs");
    }
}
