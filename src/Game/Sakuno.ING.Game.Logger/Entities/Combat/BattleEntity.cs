using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger.Entities.Combat
{
    public class BattleEntity : EntityBase
    {
        private readonly ILazyLoader lazyLoader;

        public BattleEntity() { }
        public BattleEntity(ILazyLoader lazyLoader) => this.lazyLoader = lazyLoader;

        public DateTimeOffset CompletionTime { get; set; }
        public MapId MapId { get; set; }
        public string MapName { get; set; }
        public int RouteId { get; set; }
        public MapEventKind EventKind { get; set; }
        public BattleKind BattleKind { get; set; }
        public CombinedFleetType CombinedFleetType { get; set; }
        public EventMapRank? MapRank { get; set; }
        public EventMapGaugeType? MapGaugeType { get; set; }
        public int? MapGaugeNumber { get; set; }
        public int? MapGaugeHP { get; set; }
        public int? MapGaugeMaxHP { get; set; }

        private BattleDetailEntity _details;
        public BattleDetailEntity Details
        {
            get => lazyLoader?.Load(this, ref _details) ?? _details;
            set => _details = value;
        }

        public BattleRank? Rank { get; set; }
        public int? AdmiralExperience { get; set; }
        public int? BaseExperience { get; set; }
        public bool? MapCleared { get; set; }
        public string EnemyFleetName { get; set; }
        public UseItemId? UseItemAcquired { get; set; }
        public ShipInfoId? ShipDropped { get; set; }
    }
}
