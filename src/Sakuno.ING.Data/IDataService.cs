using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Sakuno.ING.Data
{
    public interface IDataService
    {
        DbContextOptions<TContext> ConfigureDbContext<TContext>(string name) where TContext : DbContext;
        Task<Stream> ReadFile(string filename);
        Task<Stream> WriteFile(string filename);
    }
}
