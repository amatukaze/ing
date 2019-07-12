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
    public class ExpeditionCompletionVM : ITimedEntity
    {
        private readonly ExpeditionCompletionLogsVM owner;
        private readonly ExpeditionCompletionEntity entity;

        internal ExpeditionCompletionVM(ExpeditionCompletionLogsVM owner, ExpeditionCompletionEntity entity)
        {
            this.owner = owner;
            this.entity = entity;
            Expedition = owner.masterData.Expeditions[entity.ExpeditionId];
        }

        public DateTimeOffset TimeStamp => entity.TimeStamp;
        public ExpeditionInfo Expedition { get; }
        public string Result => entity.Result switch
        {
            ExpeditionResult.Fail => owner.fail,
            ExpeditionResult.Success => owner.success,
            ExpeditionResult.GreatSuccess => owner.greatSuccess,
            _ => null
        };
        public Materials MaterialsAcquired => entity.MaterialsAcquired;
        public UseItemRecord? RewardItem1 => entity.RewardItem1;
        public UseItemRecord? RewardItem2 => entity.RewardItem2;
    }

    [Export(typeof(ExpeditionCompletionLogsVM), SingleInstance = false)]
    public class ExpeditionCompletionLogsVM : LogsVM<ExpeditionCompletionVM>
    {
        private readonly Logger logger;
        private readonly ILocalizationService localization;
        internal readonly MasterDataRoot masterData;
        internal readonly string success, fail, greatSuccess;

        public ExpeditionCompletionLogsVM(Logger logger, NavalBase navalBase, ILocalizationService localization)
        {
            this.logger = logger;
            this.localization = localization;
            masterData = navalBase.MasterData;
            success = localization.GetLocalized("GameModel", "Success");
            greatSuccess = localization.GetLocalized("GameModel", "GreatSuccess");
            fail = localization.GetLocalized("GameModel", "Fail");
        }

        private protected override FilterVM<ExpeditionCompletionVM>[] CreateFilters() => new[]
            {
                new FilterVM<ExpeditionCompletionVM>(localization.GetLocalized("GameModel", "Result"),
                    x => x.Result.GetHashCode(),
                    x => x.Result),
                new FilterVM<ExpeditionCompletionVM>(localization.GetLocalized("GameModel", "Expedition"),
                    x => x.Expedition.Id,
                    x => x.Expedition.Name.Origin)
            };
        private protected override IReadOnlyCollection<ExpeditionCompletionVM> GetEntities()
        {
            if (!logger.PlayerLoaded) return Array.Empty<ExpeditionCompletionVM>();
            using (var context = logger.CreateContext())
                return context.ExpeditionCompletionTable.AsEnumerable()
                    .Select(e => new ExpeditionCompletionVM(this, e)).ToList();
        }
    }
}
