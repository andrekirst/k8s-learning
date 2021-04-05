using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace Infrastructure.Api.MediatR
{
    public class ValidationException : Exception
    {
        public ValidationException(IReadOnlyCollection<ValidationFailure> validationFailures)
            : base(GetFirstErrorMessage(validationFailures))
        {
            ValidationFailures = validationFailures;
        }

        public ValidationException(string message)
            : base(message)
        {
        }

        public IEnumerable<ValidationFailure>? ValidationFailures { get; private set; }

        private static string GetFirstErrorMessage(IEnumerable<ValidationFailure>? validationFailures)
        {
            var failures = validationFailures?.ToList() ?? new List<ValidationFailure>();
            return failures.Any() ? failures.First().ErrorMessage : "";
        }
    }
}