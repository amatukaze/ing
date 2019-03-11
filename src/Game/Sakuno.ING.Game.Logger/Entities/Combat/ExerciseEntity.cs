using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Sakuno.ING.Game.Models.Combat;

namespace Sakuno.ING.Game.Logger.Entities.Combat
{
    public class ExerciseEntity : EntityBase
    {
        private readonly ILazyLoader lazyLoader;

        public ExerciseEntity() { }
        public ExerciseEntity(ILazyLoader lazyLoader) => this.lazyLoader = lazyLoader;

        private BattleDetailEntity _details;
        internal BattleDetailEntity Details
        {
            get => lazyLoader?.Load(this, ref _details) ?? _details;
            private set => _details = value;
        }

        public int EnemyId { get; set; }
        public int EnemyLevel { get; set; }
        public string EnemyName { get; set; }
        public BattleRank? Rank { get; set; }
        public int? AdmiralExperience { get; set; }
        public int? BaseExperience { get; set; }
        public string EnemyFleetName { get; set; }

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
        public string FirstBattleDetail
        {
            get => Details?.FirstBattleDetail;
            set => EnsureDetail().FirstBattleDetail = value;
        }

        [NotMapped]
        public string SecondBattleDetail
        {
            get => Details?.SecondBattleDetail;
            set => EnsureDetail().SecondBattleDetail = value;
        }
        #endregion
    }
}
