using System.Threading;
using System.Threading.Tasks;
using Hosting.Domain.Commands;
using Hosting.Infrastructure.MediatR;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hosting.Controllers
{
    [Route("api/shorten-url")]
    [ApiController]
    public class ShortenUrlController : BaseController
    {
        public ShortenUrlController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpPost]
        public async Task<IActionResult> ShortenUrl(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] ShortenUrlRequestModel model,
            CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync(new ShortenUrlCommand(model.Url), cancellationToken);
    }
}
