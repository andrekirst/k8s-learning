﻿namespace Hosting.Infrastructure.MediatR.Results
{
    public class OkObjectResult : IRequestResult
    {
        public OkObjectResult(object value)
        {
            Value = value;
        }

        public object Value { get; set; }
    }
}