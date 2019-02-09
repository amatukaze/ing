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

        public DbSet<ShipCreationEntity> ShipCreationTable { get; private set; }
        public DbSet<EquipmentCreationEntity> EquipmentCreationTable { get; private set; }
        public DbSet<ExpeditionCompletionEntity> ExpeditionCompletionTable { get; private set; }
        internal DbSet<JNameEntity> JNameTable { get; private set; }
        public DbSet<BattleEntity> BattleTable { get; private set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<ShipCreationEntity>()
                .Property(x => x.TimeStamp)
                .HasConversion(new DateTimeOffsetToBinaryConverter());
            modelBuilder
                .Entity<ShipCreationEntity>()
                .Property(x => x.ShipBuilt)
                .HasConversion<int>(v => v, v => (ShipInfoId)v);
            modelBuilder
                .Entity<ShipCreationEntity>()
                .Property(x => x.Secretary)
                .HasConversion<int>(v => v, v => (ShipInfoId)v);

            modelBuilder
                .Entity<EquipmentCreationEntity>()
                .Property(x => x.TimeStamp)
                .HasConversion(new DateTimeOffsetToBinaryConverter());
            modelBuilder
                .Entity<EquipmentCreationEntity>()
                .Property(x => x.EquipmentCreated)
                .HasConversion<int>(v => v, v => (EquipmentInfoId)v);
            modelBuilder
                .Entity<EquipmentCreationEntity>()
                .Property(x => x.Secretary)
                .HasConversion<int>(v => v, v => (ShipInfoId)v);

            modelBuilder
                .Entity<ExpeditionCompletionEntity>()
                .Property(x => x.TimeStamp)
                .HasConversion(new DateTimeOffsetToBinaryConverter());
            modelBuilder
                .Entity<ExpeditionCompletionEntity>()
                .Property(x => x.ExpeditionId)
                .HasConversion<int>(v => v, v => (ExpeditionId)v);

            modelBuilder
                .Entity<JNameEntity>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder
                .Entity<JNameEntity>()
                .HasData(new JNameEntity { Id = 1, Name = string.Empty });

            modelBuilder
                .Entity<BattleEntity>()
                .Property(x => x.TimeStamp)
                .HasConversion(new DateTimeOffsetToBinaryConverter());
            modelBuilder
                .Entity<BattleEntity>()
                .HasOne(x => x.Details)
                .WithOne()
                .HasForeignKey<BattleDetailEntity>(x => x.TimeStamp);
            modelBuilder
                .Entity<BattleEntity>()
                .Property(x => x.MapId)
                .HasConversion<int>(v => v, v => (MapId)v);
            modelBuilder
                .Entity<BattleEntity>()
                .HasIndex(x => x.MapId);
            modelBuilder
                .Entity<BattleEntity>()
                .Property(x => x.ShipDropped)
                .HasConversion<int?>(v => v, v => (ShipInfoId?)v);
            modelBuilder
                .Entity<BattleEntity>()
                .Property(x => x.UseItemAcquired)
                .HasConversion<int?>(v => v, v => (UseItemId?)v);

            modelBuilder
                .Entity<BattleDetailEntity>()
                .Property(x => x.TimeStamp)
                .HasConversion(new DateTimeOffsetToBinaryConverter());
        }
    }
}
