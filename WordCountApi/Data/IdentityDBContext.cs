using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.EntityFrameworkCore;
using WordCountApi.Model;

namespace WordCountApi.Data
{
    public class IdentityDBContext :IdentityDbContext
    {
        //public IdentityDBContext(DbContextOptions<IdentityDBContext> options) : base(options)
        //{
                
        //}

        //public DbSet<WordCountResult> WordCountResults { get; set; }
    }
}
