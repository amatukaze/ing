using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Logger;
using Sakuno.ING.Game.Logger.Entities.Combat;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Localization;
using Sakuno.ING.Shell;

namespace Sakuno.ING.ViewModels.Logging
{
    public class BattleVM : ITimedEntity
    {
        private readonly BattleLogsVM owner;
        private readonly BattleEntity entity;
        internal BattleVM(BattleLogsVM owner, BattleEntity entity)
        {
            this.owner = owner;
            this.entity = entity;
            Map = owner.masterData.MapInfos[entity.MapId];
            Drop = owner.masterData.ShipInfos[entity.ShipDropped];
            UseItemDrop = owner.masterData.UseItems[entity.UseItemAcquired];
        }

        public DateTimeOffset TimeStamp => entity.TimeStamp;
        public MapInfo Map { get; }
        public string MapName => entity.MapName;
        public int RouteId => entity.RouteId;
        public string WinRank
        {
            get
            {
                switch (entity.Rank)
                {
                    case BattleRank.Perfect:
                        return owner.rankPerfect;
                    case BattleRank.S:
                        return owner.rankS;
                    case BattleRank.A:
                        return owner.rankA;
                    case BattleRank.B:
                        return owner.rankB;
                    case BattleRank.C:
                        return owner.rankC;
                    case BattleRank.D:
                        return owner.rankD;
                    case BattleRank.E:
                        return owner.rankE;
                    default:
                        return null;
                }
            }
        }
        public string EnemyFleetName => entity.EnemyFleetName;
        public ShipInfo Drop { get; }
        public UseItemInfo UseItemDrop { get; }

        //public void LoadDetail() => owner.shell.ShowViewWithParameter("BattleDetail", entity);
    }

    [Export(typeof(BattleLogsVM))]
    public class BattleLogsVM : LogsVM<BattleVM>
    {
        private readonly Logger logger;
        private readonly ILocalizationService localization;
        internal readonly IShell shell;
        internal readonly MasterDataRoot masterData;
        internal readonly string rankPerfect, rankS, rankA, rankB, rankC, rankD, rankE;

        public BattleLogsVM(Logger logger, NavalBase navalBase, ILocalizationService localization, IShell shell)
        {
            this.logger = logger;
            masterData = navalBase.MasterData;
            this.localization = localization;
            this.shell = shell;
            rankPerfect = "SS";
            rankS = "S";
            rankA = "A";
            rankB = "B";
            rankC = "C";
            rankD = "D";
            rankE = "E";
        }

        private protected override FilterVM<BattleVM>[] CreateFilters()
            => new[]
            {
                new FilterVM<BattleVM>("Map",
                    x => x.Map.Id,
                    x => x.Map.Id.ToString()),
                new FilterVM<BattleVM>("WinRank",
                    x => x.WinRank.GetHashCode(),
                    x => x.WinRank),
            };
        private protected override IReadOnlyCollection<BattleVM> GetEntities()
        {
            if (!logger.PlayerLoaded) return Array.Empty<BattleVM>();
            using (var context = logger.CreateContext())
                return context.BattleTable.AsEnumerable()
                    .Select(e => new BattleVM(this, e)).ToList();
        }
    }
}
