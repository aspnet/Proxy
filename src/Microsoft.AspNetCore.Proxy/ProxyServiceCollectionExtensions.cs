
using System;
using Microsoft.AspNetCore.Proxy;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ProxyServiceCollectionExtensions
    {
        public static IServiceCollection AddProxy(this IServiceCollection services)
        {
            return services.AddSingleton(new ProxyService(new SharedProxyOptions()));
        }

        public static IServiceCollection AddProxy(this IServiceCollection services, Action<SharedProxyOptions> configureOptions)
        {
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            var options = new SharedProxyOptions();
            configureOptions(options);
            return services.AddSingleton(new ProxyService(options));
        }
    }
}
