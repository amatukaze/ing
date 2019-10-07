using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Logger;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Localization;

namespace Sakuno.ING.ViewModels.Logging
{
    public class EquipmentCreationVM : ITimedEntity
    {
        private readonly EquipmentCreationEntity entity;

        internal EquipmentCreationVM(EquipmentCreationLogsVM owner, EquipmentCreationEntity entity)
        {
            this.entity = entity;
            EquipmentCreated = owner.masterData.EquipmentInfos[entity.EquipmentCreated];
            Secretary = owner.masterData.ShipInfos[entity.Secretary];
        }

        public DateTimeOffset TimeStamp => entity.TimeStamp;
        public Materials Consumption => entity.Consumption;
        public bool IsSuccess => entity.IsSuccess;
        public EquipmentInfo EquipmentCreated { get; }
        public ShipInfo Secretary { get; }
        public int SecretaryLevel => entity.SecretaryLevel;
        public int AdmiralLevel => entity.AdmiralLevel;
    }

    [Export(typeof(EquipmentCreationLogsVM), SingleInstance = false)]
    public class EquipmentCreationLogsVM : LogsVM<EquipmentCreationVM>
    {
        private readonly Logger logger;
        private readonly ILocalizationService localization;
        internal readonly MasterDataRoot masterData;
        internal readonly string success, fail;

        public EquipmentCreationLogsVM(Logger logger, NavalBase navalBase, ILocalizationService localization)
        {
            this.logger = logger;
            this.localization = localization;
            masterData = navalBase.MasterData;
            success = localization.GetLocalized("GameModel", "Success");
            fail = localization.GetLocalized("GameModel", "Fail");
        }

        private protected override FilterVM<EquipmentCreationVM>[] CreateFilters() => new[]
            {
                new FilterVM<EquipmentCreationVM>(localization.GetLocalized("GameModel", "Success"),
                    x => x.IsSuccess ? 0 : 1,
                    x => x.IsSuccess ? success : fail),
                new FilterVM<EquipmentCreationVM>(localization.GetLocalized("GameModel", "Result"),
                    x => x.EquipmentCreated?.Id ?? 0,
                    x => x.EquipmentCreated?.Name.Origin),
                new FilterVM<EquipmentCreationVM>(localization.GetLocalized("GameModel", "Secretary"),
                    x => x.Secretary.Id,
                    x => x.Secretary.Name.FullName.Origin,
                    x => new[]
                    {
                        x.Secretary.Name.FullName.Origin,
                        x.Secretary.Name.Phonetic
                    })
            };

        private protected override IReadOnlyCollection<EquipmentCreationVM> GetEntities()
        {
            if (!logger.PlayerLoaded) return Array.Empty<EquipmentCreationVM>();
            using var context = logger.CreateContext();
            return context.EquipmentCreationTable.AsEnumerable()
                .Select(e => new EquipmentCreationVM(this, e)).ToList();
        }
    }
}
