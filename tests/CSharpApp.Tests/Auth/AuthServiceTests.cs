using System.Net;
using System.Text.Json;
using CSharpApp.Application.Auth;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Interfaces;
using CSharpApp.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace CSharpApp.Tests.Auth;

public class AuthServiceTests
{
    private static AuthService CreateService(
        HttpMessageHandler handler,
        Mock<ITokenProvider>? tokenProvider = null)
    {
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.com")
        };

        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(x => x.CreateClient("RestApi")).Returns(client);

        var settings = Options.Create(new RestApiSettings
        {
            Auth = "/auth/login",
            Username = "test@example.com",
            Password = "password"
        });

        return new AuthService(
            factory.Object,
            settings,
            Mock.Of<ILogger<AuthService>>(),
            (tokenProvider ?? new Mock<ITokenProvider>()).Object);
    }

    private static Mock<HttpMessageHandler> CreateHandler(
        HttpStatusCode status, object? content = null)
    {
        var handler = new Mock<HttpMessageHandler>();

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = status,
                Content = new StringContent(
                    content is null ? "" : JsonSerializer.Serialize(content))
            });

        return handler;
    }

    // Verifies successful login and token storage.
    [Fact]
    public async Task Login_ReturnsAuthResponse()
    {
        var response = new AuthResponse { AccessToken = "test-token" };
        var tokenProvider = new Mock<ITokenProvider>();

        var service = CreateService(
            CreateHandler(HttpStatusCode.OK, response).Object,
            tokenProvider);

        var result = await service.Login();

        Assert.Equal("test-token", result.AccessToken);
        tokenProvider.Verify(x => x.SetToken("test-token"), Times.Once);
    }

    // Verifies error handling when login fails.
    [Fact]
    public async Task Login_Throws_WhenRequestFails()
    {
        var service = CreateService(
            CreateHandler(HttpStatusCode.Unauthorized).Object);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.Login());
    }

    // Verifies successful profile retrieval.
    [Fact]
    public async Task GetProfile_ReturnsProfile()
    {
        var service = CreateService(
            CreateHandler(HttpStatusCode.OK, new Profile()).Object);

        var result = await service.GetProfile();

        Assert.NotNull(result);
    }

    // Verifies error handling when profile retrieval fails.
    [Fact]
    public async Task GetProfile_Throws_WhenRequestFails()
    {
        var service = CreateService(
            CreateHandler(HttpStatusCode.Unauthorized).Object);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.GetProfile());
    }
}