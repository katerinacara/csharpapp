using CSharpApp.Application.Products;
using CSharpApp.Core.Interfaces;
using CSharpApp.Core.Settings;
using FluentValidation;
using Polly;

namespace CSharpApp.Infrastructure.Configuration;

public static class HttpConfiguration
{
    public static IServiceCollection AddHttpConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var httpSettings = configuration
           .GetSection(nameof(HttpClientSettings))
           .Get<HttpClientSettings>()!;

        services.AddHttpClient("RestApi")
            .AddTransientHttpErrorPolicy(policy =>
                policy.WaitAndRetryAsync(
                    httpSettings.RetryCount,
                    retryAttempt => TimeSpan.FromMilliseconds(
                        httpSettings.SleepDuration)));

        return services;
    }
}