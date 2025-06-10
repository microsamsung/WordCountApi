using Microsoft.EntityFrameworkCore;
using WordCountApi.Model;

namespace WordCountApi.Data
{
    public class WordDbContext : DbContext
    {
        public WordDbContext(DbContextOptions<WordDbContext> options)
            : base(options) { }

        public DbSet<Word> Words => Set<Word>();
    }
}
