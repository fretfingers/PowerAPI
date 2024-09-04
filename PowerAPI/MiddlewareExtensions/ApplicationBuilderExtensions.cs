using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PowerAPI.Data.IRepository;

namespace PowerAPI.MiddlewareExtensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseSqlTableDependency<T>(this IApplicationBuilder applicationBuilder, string connectionString)
            where T : ISubscribeTableDependency
        {
            var serviceProvider = applicationBuilder.ApplicationServices;
            var service = serviceProvider.GetService<T>();
            service.SubscribeTableDependency(connectionString);
        }
    }
}
