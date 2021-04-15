using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Hosting.Domain.Database;
using Hosting.Infrastructure;
using Hosting.Infrastructure.MediatR;
using Hosting.Services;
using MediatR;

namespace Hosting.Domain.Commands
{
    public class ShortenUrl : ICommandRequest
    {
        public string Url { get; }

        public ShortenUrl(string url)
        {
            Url = url;
        }
    }

    public class ShortenUrlHandler : IRequestHandler<ShortenUrl, IRequestResult>
    {
        public ShortenUrlHandler(
            IUrlHasher urlHasher,
            IShortenUrlRepository shortenUrlRepository,
            ICodeServiceClient codeServiceClient)
        {
        }

        public async Task<IRequestResult> Handle(ShortenUrl request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }

    public class ShortenUrlRequestModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Url can not be empty.")]
        public string Url { get; set; }
    }
}