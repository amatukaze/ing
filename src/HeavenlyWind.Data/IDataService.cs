using Microsoft.EntityFrameworkCore;

namespace Sakuno.KanColle.Amatsukaze.Data
{
    public interface IDataService
    {
        void Configure(DbContextOptionsBuilder builder);
    }
}
