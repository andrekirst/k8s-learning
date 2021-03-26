using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Api.MediatR
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiMediatR(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            var assemblyList = assemblies.ToList();

            Guard.Against.NullOrEmpty(assemblyList, nameof(assemblyList));

            services.AddMediatR(assemblyList, configuration => { });
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PipelineValidationBehavior<,>));

            return services;
        }
    }
}