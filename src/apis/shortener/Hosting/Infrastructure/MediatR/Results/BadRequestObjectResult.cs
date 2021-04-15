﻿using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hosting.Infrastructure.MediatR.Results
{
    public class BadRequestObjectResult : IRequestResult
    {
        public ModelStateDictionary? ModelState { get; set; }
        public object? Error { get; set; }

        public BadRequestObjectResult()
        {
        }

        public BadRequestObjectResult(object error)
        {
            Error = error;
        }

        public BadRequestObjectResult(ModelStateDictionary modelState)
        {
            ModelState = modelState;
        }
    }
}