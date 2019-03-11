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
        public BattleDetailEntity Details
        {
            get => lazyLoader?.Load(this, ref _details) ?? _details;
            set => _details = value;
        }

        public int EnemyId { get; set; }
        public int EnemyLevel { get; set; }
        public string EnemyName { get; set; }
        public BattleRank? Rank { get; set; }
        public int? AdmiralExperience { get; set; }
        public int? BaseExperience { get; set; }
        public string EnemyFleetName { get; set; }
    }
}
