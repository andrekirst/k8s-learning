using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using Libraries.Extensions.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using System;
using Infrastructure.Api.MediatR;

namespace Hosting
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceName", "Api")
                .Enrich.WithProperty("ServiceVersion", "v1")
                .Enrich.WithExceptionDetails()
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApiMediatR(new[] { typeof(Startup).Assembly });

            services.AddHealthChecks()
                .AddCheck<HealthCheck>("api");

            services
                .AddControllers()
                .AddFluentValidation(config =>
                {
                    // TODO Einbindung der anderen Assemblies
                    config.RegisterValidatorsFromAssemblyContaining<Startup>();
                    config.RunDefaultMvcValidationAfterFluentValidationExecutes = true;
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hosting", Version = "v1" });
            });

            services.AddProblemDetails(options =>
            {
                options.IncludeExceptionDetails = (httpContext, _) => httpContext.RequestServices.GetService<IWebHostEnvironment>().IsDevelopment();
                options.GetTraceId = context => context.TraceIdentifier;
                options.Map<ArgumentException>((_, exception) => exception.ToStatusCodeProblemDetails(StatusCodes.Status400BadRequest));
                options.Map<ArgumentNullException>((_, exception) => exception.ToStatusCodeProblemDetails(StatusCodes.Status400BadRequest));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();
            app.UseSerilogRequestLogging(options =>
            {
                options.GetLevel = (_, _, _) => LogEventLevel.Debug;
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                };
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hosting v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
