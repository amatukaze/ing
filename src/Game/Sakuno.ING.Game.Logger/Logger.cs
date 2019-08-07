using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sakuno.ING.Composition;
using Sakuno.ING.Data;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Logger.Entities.Combat;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Logger
{
    [Export(typeof(Logger), LazyCreate = false)]
    public class Logger
    {
        private readonly IDataService dataService;
        private readonly NavalBase navalBase;
        private readonly IStatePersist statePersist;
        private ShipCreationEntity shipCreation;
        private BuildingDockId lastBuildingDock;

        private readonly object admiralLock = new object();
        private Admiral currentAdmiral;

        private LoggerContext currentBattleContext;
        private HomeportFleet currentFleetInBattle, currentFleet2InBattle;
        private CombinedFleetType currentCombinedFleet;
        private BattleEntity currentBattle;
        private ExerciseEntity currentExercise;

        public Logger(IDataService dataService, GameProvider provider, NavalBase navalBase, IStatePersist statePersist)
        {
            this.dataService = dataService;
            this.navalBase = navalBase;
            this.statePersist = statePersist;

            provider.EquipmentCreated += (t, m) =>
            {
                using var context = CreateContext();
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
                {
                    using var context = CreateContext();
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
                using var context = CreateContext();
                var fleet = this.navalBase.Fleets[m.FleetId];
                context.ExpeditionCompletionTable.Add(new ExpeditionCompletionEntity
                {
                    TimeStamp = t,
                    ExpeditionId = fleet.Expedition.Id,
                    ExpeditionName = m.ExpeditionName,
                    Result = m.Result,
                    MaterialsAcquired = m.MaterialsAcquired,
                    RewardItem1 = m.RewardItem1,
                    RewardItem2 = m.RewardItem2
                });
                context.SaveChanges();
                foreach (var ship in fleet.HomeportShips)
                    this.statePersist.ClearLastSortie(ship.Id);
                this.statePersist.SaveChanges();
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

            navalBase.HomeportUpdated += (t, n) =>
            {
                var consumption =
                    currentFleetInBattle.RepairingCost +
                    currentFleetInBattle.SupplyingCost +
                    (currentFleet2InBattle?.RepairingCost ?? default) +
                    (currentFleet2InBattle?.SupplyingCost ?? default);
                var diff = consumption - this.statePersist.ConsumptionBeforeSortie;
                if (diff != default && this.statePersist.LastSortieTime is DateTimeOffset last)
                    currentBattleContext.BattleConsumptionTable.Add(new BattleConsumptionEntity
                    {
                        TimeStamp = last,
                        Consumption = diff
                    });

                currentBattle = null;
                currentExercise = null;
                currentBattleContext?.Dispose();
                currentFleetInBattle = null;
                currentFleet2InBattle = null;

                this.statePersist.LastSortieFleets = null;
                this.statePersist.LastSortieTime = null;
                this.statePersist.SaveChanges();
            };

            navalBase.ShipSupplying += (t, s, raw) =>
            {
                if (this.statePersist.GetLastSortie(s.Id) is DateTimeOffset last)
                {
                    using var context = CreateContext();
                    var entity = context.BattleConsumptionTable.Find(last);
                    if (entity is null) return;

                    int fuel = raw.CurrentFuel - s.Fuel.Current;
                    int bullet = raw.CurrentBullet - s.Bullet.Current;
                    bool isMarriaged = s.Leveling.Level >= 100;
                    entity.ActualConsumption += new Materials
                    {
                        Fuel = isMarriaged ? (int)(fuel * 0.85) : fuel,
                        Bullet = isMarriaged ? (int)(bullet * 0.85) : bullet,
                        Bauxite = (raw.SlotAircraft.Sum() - s.Slots.Sum(x => x.Aircraft.Current)) * 5
                    };
                    context.Update(entity);
                    context.SaveChanges();
                }
            };

            navalBase.ShipRepairing += (t, s, i) =>
            {
                if (this.statePersist.GetLastSortie(s.Id) is DateTimeOffset last)
                {
                    using var context = CreateContext();
                    var entity = context.BattleConsumptionTable.Find(last);
                    if (entity is null) return;

                    entity.ActualConsumption += s.RepairingCost;
                    if (i)
                        entity.ActualConsumption += new Materials
                        {
                            InstantRepair = 1
                        };
                    context.Update(entity);
                    context.SaveChanges();
                }
            };
            navalBase.RepairingDockInstant += (t, d, s) =>
            {
                if (this.statePersist.GetLastSortie(s.Id) is DateTimeOffset last)
                {
                    using var context = CreateContext();
                    var entity = context.BattleConsumptionTable.Find(last);
                    if (entity is null) return;


                    entity.ActualConsumption += new Materials
                    {
                        InstantRepair = 1
                    };
                    context.Update(entity);
                    context.SaveChanges();
                }
            };

            provider.SortieStarting += (t, m) =>
            {
                FleetId[] fleets;
                currentFleetInBattle = this.navalBase.Fleets[m.FleetId];
                currentCombinedFleet = this.navalBase.CombinedFleet;
                if (currentCombinedFleet != CombinedFleetType.None)
                {
                    currentFleet2InBattle = this.navalBase.Fleets[(FleetId)2];
                    fleets = new[] { (FleetId)1, (FleetId)2 };
                }
                else
                {
                    fleets = new[] { m.FleetId };
                }
                currentBattleContext = CreateContext();

                this.statePersist.ConsumptionBeforeSortie =
                    currentFleetInBattle.RepairingCost +
                    currentFleetInBattle.SupplyingCost +
                    (currentFleet2InBattle?.RepairingCost ?? default) +
                    (currentFleet2InBattle?.SupplyingCost ?? default);
                this.statePersist.LastSortieTime = t;
                this.statePersist.LastSortieFleets = fleets;
                foreach (var ship in currentFleetInBattle.HomeportShips)
                    this.statePersist.SetLastSortie(ship.Id, t);
                if (currentFleet2InBattle != null)
                    foreach (var ship in currentFleet2InBattle.HomeportShips)
                        this.statePersist.SetLastSortie(ship.Id, t);
                this.statePersist.SaveChanges();
            };

            provider.MapRouting += (t, m) =>
            {
                var map = this.navalBase.Maps[m.MapId];
                currentBattle = new BattleEntity
                {
                    TimeStamp = t,
                    CompletionTime = t,
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
                    MapGaugeMaxHP = map.Gauge?.Max
                };
                if (m.UnparsedLandBaseDefence != null)
                    currentBattle.LandBaseDefence = m.UnparsedLandBaseDefence.ToString(Formatting.None);

                currentBattleContext.BattleTable.Add(currentBattle);
                currentBattleContext.SaveChanges();
            };

            provider.ExerciseCandidateSelected += (t, m) =>
            {
                currentExercise = new ExerciseEntity
                {
                    TimeStamp = t,
                    EnemyId = m.AdmiralId,
                    EnemyName = m.Name,
                    EnemyLevel = m.Leveling.Level
                };
            };

            provider.ExerciseStarted += (t, m) =>
            {
                currentFleetInBattle = this.navalBase.Fleets[m];
                currentBattleContext = CreateContext();
            };

            provider.BattleStarted += (t, m) =>
            {
                if (currentBattle != null)
                {
                    currentBattle.CompletionTime = t;
                    currentBattle.SortieFleetState = currentFleetInBattle.Ships.Select(x => new ShipInBattleEntity(x)).ToArray();
                    currentBattle.SortieFleet2State = currentFleet2InBattle?.Ships.Select(x => new ShipInBattleEntity(x)).ToArray();
                    currentBattle.FirstBattleDetail = m.Unparsed.ToString(Formatting.None);
                    currentBattle.LbasState = m.Parsed.LandBasePhases
                        .Select(x => new AirForceInBattle(this.navalBase.AirForce[(currentBattle.MapId.AreaId, x.GroupId)]))
                        .ToArray();
                }
                else if (currentExercise != null)
                {
                    currentExercise.SortieFleetState = currentFleetInBattle.Ships.Select(x => new ShipInBattleEntity(x)).ToArray();
                    currentExercise.FirstBattleDetail = m.Unparsed.ToString(Formatting.None);
                    currentBattleContext.ExerciseTable.Add(currentExercise);
                }

                currentBattleContext.ChangeTracker.DetectChanges();
                currentBattleContext.SaveChanges();
            };

            provider.BattleAppended += (t, m) =>
            {
                if (currentBattle != null)
                {
                    currentBattle.CompletionTime = t;
                    currentBattle.SecondBattleDetail = m.Unparsed.ToString(Formatting.None);
                }
                else if (currentExercise != null)
                    currentExercise.SecondBattleDetail = m.Unparsed.ToString(Formatting.None);

                currentBattleContext.ChangeTracker.DetectChanges();
                currentBattleContext.SaveChanges();
            };

            provider.BattleCompleted += (t, m) =>
            {
                if (currentBattle != null)
                {
                    currentBattle.CompletionTime = t;
                    currentBattle.Rank = m.Rank;
                    currentBattle.AdmiralExperience = m.AdmiralExperience;
                    currentBattle.BaseExperience = m.BaseExperience;
                    currentBattle.MapCleared = m.MapCleared;
                    currentBattle.EnemyFleetName = m.EnemyFleetName;
                    currentBattle.UseItemAcquired = m.UseItemAcquired;
                    currentBattle.ShipDropped = m.ShipDropped;
                }
                else if (currentExercise != null)
                {
                    currentExercise.Rank = m.Rank;
                    currentExercise.AdmiralExperience = m.AdmiralExperience;
                    currentExercise.BaseExperience = m.BaseExperience;
                    currentExercise.EnemyFleetName = m.EnemyFleetName;
                }

                currentBattleContext.ChangeTracker.DetectChanges();
                currentBattleContext.SaveChanges();
            };
        }

        private void InitializeAdmiral(Admiral admiral)
        {
            using (var context = new LoggerContext(ConfigureContext(admiral?.Id)))
                context.Database.Migrate();
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
                return new LoggerContext(ConfigureContext(currentAdmiral?.Id));
        }

        private DbContextOptions<LoggerContext> ConfigureContext(int? admiralId)
            => dataService.ConfigureDbContext<LoggerContext>(admiralId?.ToString() ?? "0", "logs");
    }
}
