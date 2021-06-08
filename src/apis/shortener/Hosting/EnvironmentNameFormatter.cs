using MassTransit.Topology;
using Microsoft.AspNetCore.Hosting;

namespace Hosting
{
    public class EnvironmentNameFormatter : IEntityNameFormatter
    {
        private readonly IEntityNameFormatter _original;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EnvironmentNameFormatter(IEntityNameFormatter original, IWebHostEnvironment webHostEnvironment)
        {
            _original = original;
            _webHostEnvironment = webHostEnvironment;
        }

        public string FormatEntityName<T>() => $"{_webHostEnvironment.EnvironmentName}:{_original.FormatEntityName<T>()}";
    }
}