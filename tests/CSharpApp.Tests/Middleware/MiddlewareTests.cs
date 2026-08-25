using CSharpApp.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace CSharpApp.Tests;

public class MiddlewareTests
{
    // Verifies that the middleware calls the next request delegate.
    [Fact]
    public async Task InvokeAsync_CallsNext()
    {
        var nextCalled = false;
        var next = new RequestDelegate(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        var middleware = new Middleware(
            next,
            Mock.Of<ILogger<Middleware>>());

        await middleware.InvokeAsync(new DefaultHttpContext());

        Assert.True(nextCalled);
    }

    // Verifies that the middleware logs even when the request fails.
    [Fact]
    public async Task InvokeAsync_LogsWhenNextThrows()
    {
        var logger = new Mock<ILogger<Middleware>>();

        var middleware = new Middleware(
            _ => throw new InvalidOperationException(),
            logger.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => middleware.InvokeAsync(new DefaultHttpContext()));

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((_, _) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}