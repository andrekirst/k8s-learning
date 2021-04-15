using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BadRequestObjectResult = Hosting.Infrastructure.MediatR.Results.BadRequestObjectResult;
using BadRequestResult = Hosting.Infrastructure.MediatR.Results.BadRequestResult;
using CreatedAtActionResult = Hosting.Infrastructure.MediatR.Results.CreatedAtActionResult;
using OkObjectResult = Hosting.Infrastructure.MediatR.Results.OkObjectResult;
using OkResult = Hosting.Infrastructure.MediatR.Results.OkResult;

namespace Hosting.Infrastructure.MediatR
{
    public class BaseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BaseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [NonAction]
        protected async Task<ActionResult> ExecuteRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
            where TResponse : IRequestResult
        {
            var requestResult = await _mediator.Send(request, cancellationToken);
            return ActionResult(requestResult);
        }

        [NonAction]
        public ActionResult ActionResult(IRequestResult result) =>
            result switch
            {
                OkResult _ => Ok(),
                OkObjectResult okObjectResult => Ok(okObjectResult.Value),
                BadRequestResult _ => BadRequest(),
                BadRequestObjectResult { Error: { } } badRequestObjectResult => BadRequest(badRequestObjectResult.Error),
                BadRequestObjectResult { ModelState: { } } badRequestObjectResult => BadRequest(badRequestObjectResult.ModelState),
                BadRequestObjectResult _ => BadRequest(),
                CreatedAtActionResult createdAtActionResult => CreatedAtAction(
                    createdAtActionResult.ActionName,
                    createdAtActionResult.ControllerName.Replace("Controller", string.Empty),
                    value: createdAtActionResult.Value,
                    routeValues: createdAtActionResult.RouteValues),
                _ => throw new NotImplementedException()
            };
    }
}