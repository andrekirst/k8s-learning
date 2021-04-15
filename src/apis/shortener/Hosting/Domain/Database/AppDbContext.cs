using Hosting.Domain.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace Hosting.Domain.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<ShortenUrl> ShortenUrls { get; set; }
    }
}