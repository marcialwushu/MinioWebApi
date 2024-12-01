using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Debugging;

namespace MinioWebApi.ServiceDefaults
{
    /// <summary>
    /// Classe de extensão para configurar o logging usando Serilog e Seq.
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// Configura o Serilog como logger global e integra o Seq.
        /// </summary>
        /// <param name="hostBuilder">Construtor do host da aplicação.</param>
        /// <returns>O hostBuilder atualizado.</returns>
        public static IHostBuilder UseSeqLogging(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((context, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration) // Lê configurações do appsettings.json
                    .Enrich.FromLogContext() // Adiciona contexto aos logs
                    .WriteTo.Console() // Logs no console
                    .WriteTo.Seq(context.Configuration["Seq:Url"] ?? "http://localhost:5341"); // Logs enviados ao Seq
            });

            return hostBuilder;
        }

        /// <summary>
        /// Configura o logging com Serilog console log no pipeline do host.
        /// </summary>
        /// <param name="builder">O builder da aplicação.</param>
        public static IHostApplicationBuilder AddStructuredLogging(this IHostApplicationBuilder builder)
        {
            // Ativar diagnóstico para erros no Serilog
            SelfLog.Enable(Console.Error);

            // Configura Serilog como logger global
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration) // Lê do appsettings.json
                .Enrich.FromLogContext() // Adiciona contexto aos logs
                .WriteTo.Console() // Logs no console
                .WriteTo.Seq(builder.Configuration["Seq:Url"] ?? "http://localhost:5341", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug) // Logs enviados ao Seq
                .CreateLogger();

            // Registra o Serilog no pipeline
            builder.Logging.AddSerilog(Log.Logger);

            //// Configura console no pipeline de logging
            //builder.Logging.AddConsole(options =>
            //{
            //    options.IncludeScopes = true;
            //    options.Format = Microsoft.Extensions.Logging.Console.ConsoleLoggerFormat.Systemd;
            //    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff";
            //    options.FormatterName = "systemd";
            //});

            return builder;
        }

        /// <summary>
        /// Configura o logger global usando Serilog.
        /// </summary>
        /// <param name="configuration">Objeto de configuração da aplicação.</param>
        /// <returns>Uma instância configurada de Serilog.</returns>
        public static LoggerConfiguration ConfigureSerilog(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration) // Lê as configurações do appsettings.json
                .Enrich.FromLogContext() // Enriquecimento contextual para logs estruturados
                .WriteTo.Console() // Logs no Console
                .WriteTo.Seq(configuration["Seq:Url"] ?? "http://localhost:5341"); // Logs enviados ao Seq
        }

        /// <summary>
        /// Inicializa o logger global e registra no Serilog.
        /// </summary>
        /// <param name="configuration">Objeto de configuração da aplicação.</param>
        public static void InitializeSerilog(IConfiguration configuration)
        {
            Log.Logger = ConfigureSerilog(configuration).CreateLogger();
        }

    }
}
