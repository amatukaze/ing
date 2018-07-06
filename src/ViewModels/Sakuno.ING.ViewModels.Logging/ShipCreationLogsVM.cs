using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Logger;
using Sakuno.ING.Game.Logger.Models;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.ViewModels.Logging
{
    [Export(typeof(ShipCreationLogsVM), SingleInstance = false)]
    public class ShipCreationLogsVM : LogsVM<ShipCreationModel>
    {
        private readonly Logger logger;
        private readonly MasterDataRoot masterData;

        public ShipCreationLogsVM(Logger logger, NavalBase navalBase)
        {
            this.logger = logger;
            masterData = navalBase.MasterData;
            Refresh();
        }

        private protected override FilterVM<ShipCreationModel>[] CreateFilters()
            => new[]
            {
                new FilterVM<ShipCreationModel>("Is LSC", x => x.IsLSC ? 0 : 1, x => x.IsLSC.ToString()),
                new FilterVM<ShipCreationModel>("Ship built", x => x.ShipBuilt.Id, x => x.ShipBuilt.Name),
                new FilterVM<ShipCreationModel>("Secretary", x => x.Secretary.Id, x => x.Secretary.Name)
            };

        private protected override IReadOnlyCollection<ShipCreationModel> GetEntities()
        {
            if (!logger.PlayerLoaded) return Array.Empty<ShipCreationModel>();
            using (var context = logger.CreateContext())
                return context.ShipCreationTable.AsEnumerable()
                    .Select(e => new ShipCreationModel(masterData.ShipInfos, e)).ToList();
        }
    }
}
