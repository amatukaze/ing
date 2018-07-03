using Microsoft.EntityFrameworkCore;

namespace Sakuno.ING.Data
{
    public interface IDataService
    {
        DbContextOptions<TContext> ConfigureDbContext<TContext>(string filename, string entityname = null) where TContext : DbContext;
    }
}
