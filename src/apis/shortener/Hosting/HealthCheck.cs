using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Hosting.Domain.Database;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Hosting
{
    public class HealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppDbContext _dbContext;

        public HealthCheck(
            IHttpClientFactory httpClientFactory,
            AppDbContext dbContext)
        {
            _httpClientFactory = httpClientFactory;
            _dbContext = dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                var codeClient = _httpClientFactory.CreateClient("code");
            }
            catch
            {
                return HealthCheckResult.Unhealthy("Could not create Http-Connection to code-function.");
            }

            var canConnectToDatabase = await _dbContext.Database.CanConnectAsync(cancellationToken);

            if (!canConnectToDatabase)
            {
                return HealthCheckResult.Unhealthy("Can not connect to Database.");
            }

            return HealthCheckResult.Healthy();
        }
    }
}