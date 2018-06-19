using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Data;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Logger
{
    [Export(typeof(Logger), LazyCreate = false)]
    internal class Logger
    {
        private readonly IDataService dataService;
        private readonly NavalBase navalBase;

        private ShipCreation shipCreation;
        private BuildingDockId lastBuildingDock;

        public Logger(IDataService dataService, IGameProvider provider, NavalBase navalBase)
        {
            this.dataService = dataService;
            this.navalBase = navalBase;

            provider.EquipmentCreated += (t, m) =>
            {
                using (var context = CreateContext())
                {
                    context.EquipmentCreationTable.Add(new EquipmentCreation
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
                shipCreation = new ShipCreation
                {
                    TimeStamp = t,
                    Consumption = m.Consumption,
                    IsLSC = m.IsLSC,
                    AdmiralLevel = this.navalBase.Admiral.Leveling.Level,
                    Secretary = this.navalBase.Secretary.Info.Id,
                    SecretaryLevel = this.navalBase.Secretary.Leveling.Level,
                    EmptyDockCount = navalBase.BuildingDocks.Count(x => x.State == BuildingDockState.Empty)
                };
                lastBuildingDock = m.BuildingDockId;
            };

            provider.BuildingDockUpdated += (t, m) =>
            {
                if (shipCreation != null)
                    using (var context = CreateContext())
                    {
                        shipCreation.ShipBuilt = m.Single(x => x.Id == lastBuildingDock).BuiltShipId.Value;
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
                    context.ExpeditionCompletionTable.Add(new ExpeditionCompletion
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
        }

        public LoggerContext CreateContext() => new LoggerContext(dataService.ConfigureDbContext<LoggerContext>("logs"));
    }
}
