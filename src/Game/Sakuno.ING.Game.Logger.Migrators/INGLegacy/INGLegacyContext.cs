using Microsoft.EntityFrameworkCore;

namespace Sakuno.ING.Game.Logger.Migrators.INGLegacy
{
    internal class INGLegacyContext : DbContext
    {
        public DbSet<Construction> ConstructionTable { get; private set; }
        public DbSet<Development> DevelopmentTable { get; private set; }
        public DbSet<Expedition> ExpeditionTable { get; private set; }

        private readonly string path;
        public INGLegacyContext(string path) => this.path = path;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("DataSource=" + path);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Construction>()
                .ToTable("construction")
                .HasKey(x => x.time);
            modelBuilder.Entity<Development>()
                .ToTable("development")
                .HasKey(x => x.time);
            modelBuilder.Entity<Expedition>()
                .ToTable("expedition")
                .HasKey(x => x.time);
        }
    }
}
