using Polly;

namespace CSharpApp.Infrastructure.Configuration;

public static class HttpConfiguration
{
    public static IServiceCollection AddHttpConfiguration(this IServiceCollection services)
    {
        services.AddHttpClient<ProductsService>().AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromMilliseconds(100)));
        return services;
    }
}