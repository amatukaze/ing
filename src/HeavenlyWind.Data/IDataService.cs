using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Sakuno.KanColle.Amatsukaze.Data
{
    public interface IDataService
    {
        void ConfigureDbContext(DbContextOptionsBuilder builder);
        Task<Stream> ReadFile(string filename);
        Task<Stream> WriteFile(string filename);
    }
}
