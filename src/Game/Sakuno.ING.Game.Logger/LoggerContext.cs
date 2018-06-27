using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger
{
    public class LoggerContext : DbContext
    {
        internal LoggerContext(DbContextOptions<LoggerContext> options) : base(options) { }

        public DbSet<ShipCreation> ShipCreationTable { get; private set; }
        public DbSet<EquipmentCreation> EquipmentCreationTable { get; private set; }
        public DbSet<ExpeditionCompletion> ExpeditionCompletionTable { get; private set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<ShipCreation>()
                .Property(x => x.TimeStamp)
                .HasConversion(new DateTimeOffsetToBinaryConverter());
            modelBuilder
                .Entity<ShipCreation>()
                .OwnsOne(x => x.Consumption,
                    c => c
                        .Ignore(m => m.InstantBuild)
                        .Ignore(m => m.InstantRepair)
                        .Ignore(m => m.Improvement));
            modelBuilder
                .Entity<ShipCreation>()
                .Property(x => x.ShipBuilt)
                .HasConversion<int>(v => v, v => (ShipInfoId)v);
            modelBuilder
                .Entity<ShipCreation>()
                .Property(x => x.Secretary)
                .HasConversion<int>(v => v, v => (ShipInfoId)v);

            modelBuilder
                .Entity<EquipmentCreation>()
                .Property(x => x.TimeStamp)
                .HasConversion(new DateTimeOffsetToBinaryConverter());
            modelBuilder
                .Entity<EquipmentCreation>()
                .OwnsOne(x => x.Consumption,
                    c => c
                        .Ignore(m => m.InstantBuild)
                        .Ignore(m => m.InstantRepair)
                        .Ignore(m => m.Improvement)
                        .Ignore(m => m.Development));
            modelBuilder
                .Entity<EquipmentCreation>()
                .Property(x => x.EquipmentCreated)
                .HasConversion<int>(v => v, v => (EquipmentInfoId)v);
            modelBuilder
                .Entity<EquipmentCreation>()
                .Property(x => x.Secretary)
                .HasConversion<int>(v => v, v => (ShipInfoId)v);

            modelBuilder
                .Entity<ExpeditionCompletion>()
                .Property(x => x.TimeStamp)
                .HasConversion(new DateTimeOffsetToBinaryConverter());
            modelBuilder
                .Entity<ExpeditionCompletion>()
                .OwnsOne(x => x.MaterialsAcquired,
                    c => c
                        .Ignore(m => m.Development)
                        .Ignore(m => m.Improvement)
                        .Ignore(m => m.InstantBuild)
                        .Ignore(m => m.InstantRepair));
            modelBuilder
                .Entity<ExpeditionCompletion>()
                .Property(x => x.ExpeditionId)
                .HasConversion<int>(v => v, v => (ExpeditionId)v);
            modelBuilder
                .Entity<ExpeditionCompletion>()
                .OwnsOne(x => x.RewardItem1,
                    rb => rb
                        .Property(r => r.ItemId)
                        .HasConversion<int>(v => v, v => (UseItemId)v));
            modelBuilder
                .Entity<ExpeditionCompletion>()
                .OwnsOne(x => x.RewardItem2,
                    rb => rb
                        .Property(r => r.ItemId)
                        .HasConversion<int>(v => v, v => (UseItemId)v));
        }
    }
}
