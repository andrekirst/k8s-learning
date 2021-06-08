using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;
using System;
using System.Net.Http;
using Hosting.Domain.Database;
using Hosting.Extensions.ProblemDetails;
using Hosting.Infrastructure.MediatR;
using Hosting.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;

namespace Hosting
{
    public class Startup
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
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
            services.AddDistributedMemoryCache();
            services.AddApiMediatR(new[] { typeof(Startup).Assembly });

            services.AddSingleton<IUrlHasher, UrlHasher>();
            services.AddScoped<IShortenUrlRepository, ShortenUrlRepository>();

            services.AddHealthChecks()
                .AddCheck<HealthCheck>("api");

            services
                .AddDbContextPool<AppDbContext>(options =>
                {
                    options.UseNpgsql(Configuration["Database:ConnectionString"]);
                });

            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(3));

            services
                .AddHttpClient("code", c =>
                {
                    c.BaseAddress = new Uri(Configuration["Api:Code:Url"]);
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                {
                    UseDefaultCredentials =  true
                })
                .AddPolicyHandler(retryPolicy);
            services.AddScoped<ICodeServiceClient, CodeServiceClient>();

            services
                .AddControllers()
                .AddFluentValidation(config =>
                {
                    config.RegisterValidatorsFromAssemblyContaining<Startup>();
                    config.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
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

            services
                .AddMassTransit(c =>
                {
                    c.UsingRabbitMq((context, cfg) =>
                    {
                        var host = Configuration["Messaging:Host"] ?? "localhost";
                        var username = Configuration["Messaging:Username"] ?? "guest";
                        var password = Configuration["Messaging:Password"] ?? "guest";

                        cfg.ConfigureEndpoints(context);
                        cfg.Host(host, h =>
                        {
                            h.Password(password);
                            h.Username(username);
                        });
                        cfg.MessageTopology.SetEntityNameFormatter(new EnvironmentNameFormatter(cfg.MessageTopology.EntityNameFormatter, _webHostEnvironment));
                    });
                });

            services.AddMassTransitHostedService();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, AppDbContext appDbContext)
        {
            loggerFactory.AddSerilog();
            app.UseSerilogRequestLogging(options =>
            {
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

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            appDbContext.Database.Migrate();
        }
    }
}
