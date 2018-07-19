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
    public class ShipCreationVM : ITimedEntity
    {
        private readonly ShipCreationLogsVM owner;
        private readonly ShipCreationEntity entity;

        internal ShipCreationVM(ShipCreationLogsVM owner, ShipCreationEntity entity)
        {
            this.owner = owner;
            this.entity = entity;
            ShipBuilt = owner.masterData.ShipInfos[entity.ShipBuilt];
            Secretary = owner.masterData.ShipInfos[entity.Secretary];
        }

        public DateTimeOffset TimeStamp => entity.TimeStamp;
        public Materials Consumption => entity.Consumption;
        public bool IsLSC => entity.IsLSC;
        public string ConstructionType => IsLSC ? owner.lsc : owner.nsc;
        public ShipInfo ShipBuilt { get; }
        public int EmptyDockCount => entity.EmptyDockCount;
        public ShipInfo Secretary { get; }
        public int SecretaryLevel => entity.SecretaryLevel;
        public int AdmiralLevel => entity.AdmiralLevel;
    }

    [Export(typeof(ShipCreationLogsVM), SingleInstance = false)]
    public class ShipCreationLogsVM : LogsVM<ShipCreationVM>
    {
        private readonly Logger logger;
        private readonly ILocalizationService localization;
        internal readonly MasterDataRoot masterData;
        internal readonly string nsc, lsc;

        public ShipCreationLogsVM(Logger logger, NavalBase navalBase, ILocalizationService localization)
        {
            this.logger = logger;
            this.localization = localization;
            masterData = navalBase.MasterData;
            nsc = localization.GetLocalized("GameModel", "ConstructionType_Normal");
            lsc = localization.GetLocalized("GameModel", "ConstructionType_Large");
        }

        private protected override FilterVM<ShipCreationVM>[] CreateFilters()
        {
            return new[]
            {
                new FilterVM<ShipCreationVM>(localization.GetLocalized("GameModel", "ConstructionType"),
                    x => x.IsLSC ? 0 : 1,
                    x => x.IsLSC ? lsc : nsc),
                new FilterVM<ShipCreationVM>(localization.GetLocalized("GameModel", "Result"),
                    x => x.ShipBuilt.Id,
                    x => x.ShipBuilt.Name.Origin,
                    x => new[]
                    {
                        x.ShipBuilt.Name.Origin,
                        x.ShipBuilt.Name.Phonetic
                    }),
                new FilterVM<ShipCreationVM>(localization.GetLocalized("GameModel", "Secretary"),
                    x => x.Secretary.Id,
                    x => x.Secretary.Name.Origin,
                    x => new[]
                    {
                        x.Secretary.Name.Origin,
                        x.Secretary.Name.Phonetic
                    })
            };
        }

        private protected override IReadOnlyCollection<ShipCreationVM> GetEntities()
        {
            if (!logger.PlayerLoaded) return Array.Empty<ShipCreationVM>();
            using (var context = logger.CreateContext())
                return context.ShipCreationTable.AsEnumerable()
                    .Select(e => new ShipCreationVM(this, e)).ToList();
        }
    }
}
