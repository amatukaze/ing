using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        internal BattleDetailEntity Details
        {
            get => lazyLoader?.Load(this, ref _details) ?? _details;
            private set => _details = value;
        }

        public BattleRank? Rank { get; set; }
        public int? AdmiralExperience { get; set; }
        public int? BaseExperience { get; set; }
        public bool? MapCleared { get; set; }
        public string EnemyFleetName { get; set; }
        public UseItemId? UseItemAcquired { get; set; }
        public ShipInfoId? ShipDropped { get; set; }

        #region Agent properties
        private BattleDetailEntity EnsureDetail()
        {
            Details ??= new BattleDetailEntity();
            return Details;
        }

        [NotMapped]
        public IReadOnlyList<ShipInBattleEntity> SortieFleetState
        {
            get => Details?.SortieFleetState;
            set => EnsureDetail().SortieFleetState = value;
        }

        [NotMapped]
        public IReadOnlyList<ShipInBattleEntity> SortieFleet2State
        {
            get => Details?.SortieFleet2State;
            set => EnsureDetail().SortieFleet2State = value;
        }

        [NotMapped]
        public IReadOnlyList<ShipInBattleEntity> SupportFleetState
        {
            get => Details?.SupportFleetState;
            set => EnsureDetail().SupportFleetState = value;
        }

        [NotMapped]
        public IReadOnlyCollection<AirForceInBattle> LbasState
        {
            get => Details?.LbasState;
            set => EnsureDetail().LbasState = value;
        }

        [NotMapped]
        public string LandBaseDefence
        {
            get => Details?.LandBaseDefence;
            set
            {
                EnsureDetail().LandBaseDefence = value;
                HasLandBaseDefense = true;
            }
        }

        [NotMapped]
        public string FirstBattleDetail
        {
            get => Details?.FirstBattleDetail;
            set
            {
                EnsureDetail().FirstBattleDetail = value;
                HasBattleDetail = true;
            }
        }

        [NotMapped]
        public string SecondBattleDetail
        {
            get => Details?.SecondBattleDetail;
            set
            {
                EnsureDetail().SecondBattleDetail = value;
                HasBattleDetail = true;
            }
        }
        #endregion

        public bool HasBattleDetail { get; private set; }
        public bool HasLandBaseDefense { get; private set; }
    }
}
