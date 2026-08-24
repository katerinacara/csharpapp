using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;

namespace CSharpApp.Api.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IVersionedEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("api/v{version:apiVersion}/auth/login",
                async ([FromServices] IAuthService authService) =>
                {
                    var authResponse = await authService.Login();
                    return Results.Ok(authResponse);
                })
                .WithName("Login")
                .WithSummary("Authenticate against the external API")
                .HasApiVersion(1.0);

            endpoints.MapGet("api/v{version:apiVersion}/auth/profile",
                async ([FromServices] IAuthService authService) =>
                {
                    var profile = await authService.GetProfile();
                    return Results.Ok(profile);
                })
                .WithName("GetProfile")
                .WithSummary("Get the authenticated user's profile")
                .HasApiVersion(1.0);
        }
    }
}
