using Microsoft.EntityFrameworkCore;

namespace Sakuno.ING.Data
{
    public interface IDataService
    {
        DbContextOptions<TContext> ConfigureDbContext<TContext>(string name) where TContext : DbContext;
    }
}
