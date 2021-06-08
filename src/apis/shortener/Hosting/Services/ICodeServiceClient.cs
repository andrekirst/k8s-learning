using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Hosting.Services
{
    public interface ICodeServiceClient
    {
        Task<string> CreateCode(CancellationToken cancellationToken = default);
    }

    public class CodeServiceClient : ICodeServiceClient
    {
        private readonly IHttpClientFactory _clientFactory;

        public CodeServiceClient(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> CreateCode(CancellationToken cancellationToken = default)
        {
            var client = _clientFactory.CreateClient("code");
            var code = await client.GetStringAsync("", cancellationToken);
            return code;
        }
    }
}