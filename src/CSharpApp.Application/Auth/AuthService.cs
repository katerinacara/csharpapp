using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Auth
{
    using CSharpApp.Core.Dtos;
    using CSharpApp.Core.Interfaces;
    using CSharpApp.Core.Settings;
    using Microsoft.Extensions.Options;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;

    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RestApiSettings _restApiSettings;
        private readonly ILogger<AuthService> _logger;
        private readonly ITokenProvider _tokenProvider;

        public AuthService(
            IHttpClientFactory httpClientFactory,
            IOptions<RestApiSettings> restApiSettings,
            ILogger<AuthService> logger, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _restApiSettings = restApiSettings.Value;
            _logger = logger;
            _tokenProvider = tokenProvider;
        }

        public async Task<AuthResponse> Login()
        {
            var client = _httpClientFactory.CreateClient("RestApi");

            client.BaseAddress = new Uri(_restApiSettings.BaseUrl!);

            var request = new AuthRequest
            {
                Email = _restApiSettings.Username!,
                Password = _restApiSettings.Password!
            };

            var response = await client.PostAsJsonAsync(
                _restApiSettings.Auth,
                request);

            response.EnsureSuccessStatusCode();

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>()
                ?? throw new InvalidOperationException(
                    "Authentication response could not be deserialized.");

            _tokenProvider.SetToken(authResponse.AccessToken);

            return authResponse;
        }

        public async Task<Profile> GetProfile()
        {
            var client = _httpClientFactory.CreateClient("RestApi");

            client.BaseAddress = new Uri(_restApiSettings.BaseUrl!);

            var response = await client.GetAsync("auth/profile");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Profile>()
                ?? throw new InvalidOperationException(
                    "Profile could not be deserialized.");
        }
    }
}
