using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sakuno.ING.Data;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger
{
    internal class LoggerContext : DatabaseContext
    {
        public LoggerContext(IDataService dataService) : base(dataService) { }
        protected override string Name => "logs";

        public DbSet<ShipCreation> ShipCreationTable { get; private set; }
        public DbSet<EquipmentCreation> EquipmentCreationTable { get; private set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<ShipCreation>()
                .OwnsOne(x => x.Consumption,
                    c => c
                        .Ignore(m => m.InstantBuild)
                        .Ignore(m => m.InstantRepair)
                        .Ignore(m => m.Improvement));
            modelBuilder
                .Entity<ShipCreation>()
                .Property(x => x.TimeStamp)
                .HasConversion(new DateTimeOffsetToBinaryConverter());
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
                .OwnsOne(x => x.Consumption,
                    c => c
                        .Ignore(m => m.InstantBuild)
                        .Ignore(m => m.InstantRepair)
                        .Ignore(m => m.Improvement)
                        .Ignore(m => m.Development));
            modelBuilder
                .Entity<EquipmentCreation>()
                .Property(x => x.TimeStamp)
                .HasConversion(new DateTimeOffsetToBinaryConverter());
            modelBuilder
                .Entity<EquipmentCreation>()
                .Property(x => x.EquipmentCreated)
                .HasConversion<int>(v => v, v => (EquipmentInfoId)v);
            modelBuilder
                .Entity<EquipmentCreation>()
                .Property(x => x.Secretary)
                .HasConversion<int>(v => v, v => (ShipInfoId)v);
        }
    }
}
