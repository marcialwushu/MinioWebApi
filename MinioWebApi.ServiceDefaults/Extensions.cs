using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults

/// <summary>
/// Extens�es para adicionar configura��es padr�o de servi�o � aplica��o.
/// Inclui configura��es de OpenTelemetry, health checks, resili�ncia e descoberta de servi�os. 
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adiciona servi�os padr�o, como OpenTelemetry, descoberta de servi�os e health checks.
    /// </summary>
    /// <param name="builder">Construtor da aplica��o.</param>
    /// <returns>Construtor atualizado com os servi�os padr�o.</returns>
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Adiciona resili�ncia padr�o
            http.AddStandardResilienceHandler();

            // Adiciona descoberta de servi�os
            http.AddServiceDiscovery();
        });

        return builder;
    }

    /// <summary>
    /// Configura o OpenTelemetry para coleta de m�tricas e traces.
    /// </summary>
    /// <param name="builder">Construtor da aplica��o.</param>
    /// <returns>Construtor atualizado com OpenTelemetry configurado.</returns>
    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                    // Adiciona instrumenta��o para gRPC (descomente se necess�rio)
                    //.AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    /// <summary>
    /// Configura os exportadores do OpenTelemetry, como OTLP ou Azure Monitor.
    /// </summary>
    /// <param name="builder">Construtor da aplica��o.</param>
    /// <returns>Construtor atualizado com exportadores configurados.</returns>
    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Debug"))
        {
            // Adiciona o exportador de Console para ambientes de desenvolvimento ou debug
            builder.Services.AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing.AddConsoleExporter();
                })
                .WithMetrics(metrics =>
                {
                    metrics.AddConsoleExporter();
                });

            // Construindo o provedor de servi�os para acessar o ILoggerFactory
            using var serviceProvider = builder.Services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.CreateLogger("OpenTelemetry")
                .LogInformation("Console exporter enabled for development/debug environment.");
        }

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        // Configura��o opcional para Azure Monitor
        //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //    builder.Services.AddOpenTelemetry()
        //       .UseAzureMonitor();
        //}

        return builder;
    }

    /// <summary>
    /// Adiciona verifica��es de integridade padr�o.
    /// </summary>
    /// <param name="builder">Construtor da aplica��o.</param>
    /// <returns>Construtor atualizado com health checks configurados.</returns>
    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            // Verifica��o de liveness padr�o
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    /// <summary>
    /// Configura endpoints padr�o, incluindo health checks.
    /// Dispon�vel apenas em ambientes de desenvolvimento.
    /// </summary>
    /// <param name="app">Aplica��o web.</param>
    /// <returns>Aplica��o configurada com endpoints padr�o.</returns>
    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // Health checks completos
            app.MapHealthChecks("/health");

            //  Health checks de liveness
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }


}
