using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinioWebApi.ServiceDefaults
{
    public class AspireExtensions
    {
        /// <summary>
        /// Configura padrões específicos do Aspire, como resiliência e políticas.
        /// </summary>
        /// <param name="services">Coleção de serviços da aplicação.</param>
        //public static IServiceCollection ConfigureAspireDefaults(this IServiceCollection services)
        ///{
            //// Exemplo de configuração para autenticação e autorização padrão
            //services.AddAspireAuthentication();
            //services.AddAspireAuthorization();

            //// Configura padrões para políticas de resiliência
            //services.AddAspireResilience(resilience =>
            //{
            //    resilience.AddPolicy("default", policyBuilder =>
            //    {
            //        policyBuilder.RetryAsync(3);
            //        policyBuilder.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
            //    });
            //});

            //return services;
        //}
    }
}
