using System.Threading;
using System.Threading.Tasks;
using Domain.ShortUrl.Commands;
using Infrastructure.Api.MediatR;
using JetBrains.Annotations;
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
        public async Task<IActionResult> ShortenUrl([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] ShortenUrlRequestModel model, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync(new ShortenUrl(model.Url), cancellationToken);
    }
}
