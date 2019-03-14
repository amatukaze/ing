using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Game;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Logger;
using Sakuno.ING.Game.Logger.Entities.Combat;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Localization;
using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Logging.Combat;

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
        public MapId MapId => entity.MapId;
        public string MapName => entity.MapName;
        public int RouteId => entity.RouteId;
        public BattleRank? Rank => entity.Rank;
        public string RankText => owner.FormatRank(Rank);
        public string EnemyFleetName => entity.EnemyFleetName;
        public ShipInfo Drop { get; }
        public UseItemInfo UseItemDrop { get; }
        public bool HasBattleDetail => entity.HasBattleDetail;
        public Battle Detail { get; private set; }

        public void LoadDetail()
        {
            if (!HasBattleDetail) return;
            if (Detail is null)
            {
                var battle = new Battle
                (
                    entity.SortieFleetState?.Select(x => new LoggedShip(owner.masterData, x)).ToArray(),
                    entity.SortieFleet2State?.Select(x => new LoggedShip(owner.masterData, x)).ToArray(),
                    entity.CombinedFleetType,
                    entity.BattleKind
                );
                TryAppend(battle, entity.FirstBattleDetail);
                TryAppend(battle, entity.SecondBattleDetail);
                Detail = battle;
            }
            owner.shell.ShowViewWithParameter("BattleLogDetail", this);
        }

        private void TryAppend(Battle battle, string json)
        {
            if (json is null) return;
            var api = owner.provider.Deserialize<BattleDetailJson>(json);
            var raw = new RawBattle(api, TimeStamp < RawBattle.EnemyIdChangeTime);
            battle.Append(owner.masterData, raw);
        }
    }

    [Export(typeof(BattleLogsVM), SingleInstance = false)]
    public class BattleLogsVM : LogsVM<BattleVM>, IDisposable
    {
        private readonly Logger logger;
        internal readonly GameProvider provider;
        private readonly ILocalizationService localization;
        internal readonly IShell shell;
        internal readonly MasterDataRoot masterData;
        private readonly string rankPerfect, rankS, rankA, rankB, rankC, rankD, rankE;
        private LoggerContext context;

        public BattleLogsVM(Logger logger, NavalBase navalBase, GameProvider provider, ILocalizationService localization, IShell shell)
        {
            this.logger = logger;
            this.provider = provider;
            masterData = navalBase.MasterData;
            this.localization = localization;
            this.shell = shell;
            rankPerfect = localization.GetLocalized("Combat", "BattleRank_Perfect");
            rankS = localization.GetLocalized("Combat", "BattleRank_S");
            rankA = localization.GetLocalized("Combat", "BattleRank_A");
            rankB = localization.GetLocalized("Combat", "BattleRank_B");
            rankC = localization.GetLocalized("Combat", "BattleRank_C");
            rankD = localization.GetLocalized("Combat", "BattleRank_D");
            rankE = localization.GetLocalized("Combat", "BattleRank_E");
        }

        internal string FormatRank(BattleRank? rank) => rank switch
        {
            BattleRank.Perfect => rankPerfect,
            BattleRank.S => rankS,
            BattleRank.A => rankA,
            BattleRank.B => rankB,
            BattleRank.C => rankC,
            BattleRank.D => rankD,
            BattleRank.E => rankE,
            _ => null
        };

        private protected override FilterVM<BattleVM>[] CreateFilters()
            => new[]
            {
                new FilterVM<BattleVM>(localization.GetLocalized("Combat", "MapId"),
                    x => x.MapId,
                    x => x.MapId.ToString()),
                new FilterVM<BattleVM>(localization.GetLocalized("Combat", "Rank"),
                    x => x.Rank.GetHashCode(),
                    x => x.RankText),
            };

        private protected override IReadOnlyCollection<BattleVM> GetEntities()
        {
            if (!logger.PlayerLoaded) return Array.Empty<BattleVM>();
            if (context is null)
                context = logger.CreateContext();
            return context.BattleTable.AsEnumerable()
                .Select(e => new BattleVM(this, e)).ToList();
        }

        public void Dispose()
        {
            if (context != null)
            {
                context.Dispose();
                context = null;
            }
        }
    }
}
