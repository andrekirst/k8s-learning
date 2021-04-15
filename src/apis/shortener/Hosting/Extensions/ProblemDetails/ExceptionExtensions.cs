using System;
using Hellang.Middleware.ProblemDetails;

namespace Hosting.Extensions.ProblemDetails
{
    public static class ExceptionExtensions
    {
        public static StatusCodeProblemDetails ToStatusCodeProblemDetails<TException>(this TException exception, int statusCode)
            where TException : Exception =>
            new StatusCodeProblemDetails(statusCode)
            {
                Detail = exception.Message,
                Title = exception.Message,
                Type = typeof(TException).Name
            };
    }
}
