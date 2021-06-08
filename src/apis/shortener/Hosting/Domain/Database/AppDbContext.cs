using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var shortenUrls = ChangeTracker.Entries<ShortenUrl>()
                .Where(c => c.State == EntityState.Added);

            var now = DateTime.UtcNow;

            foreach (var entityEntry in shortenUrls)
            {
                entityEntry.Entity.CreatedAt = now;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}