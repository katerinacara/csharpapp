using CSharpApp.Application.Products;
using CSharpApp.Core.Interfaces;
using CSharpApp.Core.Settings;
using CSharpApp.Infrastructure.Authentication;
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
       
        var restApiSettings = configuration
            .GetSection(nameof(RestApiSettings))
            .Get<RestApiSettings>()!;

        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddTransient<JwtAuthenticationHandler>();

        services.AddHttpClient("RestApi", client =>
        {
            client.BaseAddress = new Uri(restApiSettings.BaseUrl!);
        })
        .AddTransientHttpErrorPolicy(policy =>
            policy.WaitAndRetryAsync(
                httpSettings.RetryCount,
                retryAttempt => TimeSpan.FromMilliseconds(
                    httpSettings.SleepDuration)))
        .AddHttpMessageHandler<JwtAuthenticationHandler>();


        return services;
    }
}