using OpenTelemetry.Metrics;
using System.Diagnostics.Metrics;

namespace MinioWebApi.Metrics
{
    /// <summary>
    /// Classe para gerenciar métricas customizadas.
    /// </summary>
    public class CustomMetrics
    {
        private static readonly Meter Meter = new("MinioWebApi.Metrics", "1.0.0");

        /// <summary>
        /// Contador para total de requisições.
        /// </summary>
        public static Counter<long> RequestCounter { get; } =
            Meter.CreateCounter<long>("custom_requests_total", description: "Total de requisições customizadas");

        /// <summary>
        /// Histograma para duração das requisições.
        /// </summary>
        public static Histogram<double> RequestDuration { get; } =
            Meter.CreateHistogram<double>("custom_request_duration_seconds", description: "Duração das requisições em segundos");

        /// <summary>
        /// Incrementa o contador de requisições.
        /// </summary>
        public static void IncrementRequest()
        {
            RequestCounter.Add(1);
        }

        /// <summary>
        /// Registra a duração de uma requisição.
        /// </summary>
        /// <param name="duration">Duração da requisição em segundos.</param>
        public static void RecordDuration(double duration)
        {
            RequestDuration.Record(duration);
        }
    }
}
