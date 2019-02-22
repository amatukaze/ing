using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Logger.Entities.Combat;
using Sakuno.ING.Game.Models.MasterData;

[assembly: InternalsVisibleTo("Sakuno.ING.Game.Logger.Design")]
[assembly: InternalsVisibleTo("Sakuno.ING.Game.Logger.Tests")]
namespace Sakuno.ING.Game.Logger
{
    public class LoggerContextBase : DbContext
    {
        internal LoggerContextBase(DbContextOptions<LoggerContextBase> options) : base(options)
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<ShipCreationEntity> ShipCreationTable { get; protected set; }
        public DbSet<EquipmentCreationEntity> EquipmentCreationTable { get; protected set; }
        public DbSet<ExpeditionCompletionEntity> ExpeditionCompletionTable { get; protected set; }
        internal DbSet<JNameEntity> JNameTable { get; set; }
        public DbSet<BattleEntity> BattleTable { get; protected set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<ShipCreationEntity>(e =>
                {
                    e.Property(x => x.TimeStamp).HasConversion(new DateTimeOffsetToBinaryConverter());
                    e.Property(x => x.ShipBuilt).HasConversion<int>(v => v, v => (ShipInfoId)v);
                    e.Property(x => x.Secretary).HasConversion<int>(v => v, v => (ShipInfoId)v);
                });
            modelBuilder
                .Entity<EquipmentCreationEntity>(e =>
                {
                    e.Property(x => x.TimeStamp).HasConversion(new DateTimeOffsetToBinaryConverter());
                    e.Property(x => x.EquipmentCreated).HasConversion<int>(v => v, v => (EquipmentInfoId)v);
                    e.Property(x => x.Secretary).HasConversion<int>(v => v, v => (ShipInfoId)v);
                });
            modelBuilder
                .Entity<ExpeditionCompletionEntity>(e =>
                {
                    e.Property(x => x.TimeStamp).HasConversion(new DateTimeOffsetToBinaryConverter());
                    e.Property(x => x.ExpeditionId).HasConversion<int>(v => v, v => (ExpeditionId)v);
                });
            modelBuilder
                .Entity<JNameEntity>(e =>
                {
                    e.Property(x => x.Id).ValueGeneratedNever();
                    e.HasIndex(x => x.Name).IsUnique();
                    e.HasData(new JNameEntity { Id = 1, Name = string.Empty });
                });
            modelBuilder
                .Entity<BattleEntity>(e =>
                {
                    e.Property(x => x.TimeStamp).HasConversion(new DateTimeOffsetToBinaryConverter());
                    e.OwnsOne(x => x.Details).ToTable("BattleDetails");
                    e.Property(x => x.CompletionTime).HasConversion(new DateTimeOffsetToBinaryConverter());
                    e.Property(x => x.MapId).HasConversion<int>(v => v, v => (MapId)v);
                    e.HasIndex(x => x.MapId);
                    e.Property(x => x.ShipDropped).HasConversion<int?>(v => v, v => (ShipInfoId?)v);
                    e.Property(x => x.UseItemAcquired).HasConversion<int?>(v => v, v => (UseItemId?)v);
                });
        }
    }
}
