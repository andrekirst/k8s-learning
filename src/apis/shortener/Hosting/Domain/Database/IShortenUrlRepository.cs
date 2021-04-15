using System.Threading;
using System.Threading.Tasks;
using Hosting.Domain.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace Hosting.Domain.Database
{
    public interface IShortenUrlRepository
    {
        Task<bool> ExistsEntryByUrlHash(string urlHash, CancellationToken cancellationToken = default);
        Task CreateEntry(string code, string url, string urlHash, CancellationToken cancellationToken = default);
    }

    public class ShortenUrlRepository : IShortenUrlRepository
    {
        private readonly AppDbContext _dbContext;

        public ShortenUrlRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> ExistsEntryByUrlHash(string urlHash, CancellationToken cancellationToken = default)
            => _dbContext.ShortenUrls.AsNoTracking().AnyAsync(su => su.UrlHash == urlHash, cancellationToken);

        public async Task CreateEntry(string code, string url, string urlHash, CancellationToken cancellationToken = default)
        {
            await _dbContext.ShortenUrls.AddAsync(new ShortenUrl
            {
                Code = code,
                Url = url,
                UrlHash = urlHash
            }, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}