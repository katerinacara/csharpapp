using CSharpApp.Application.Categories;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace CSharpApp.Tests.Categories;

public class CategoryServiceTests
{
    private static CategoriesService CreateService(HttpMessageHandler handler)
    {
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.com")
        };

        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(x => x.CreateClient("RestApi")).Returns(client);

        var settings = Options.Create(new RestApiSettings
        {
            Categories = "/api/categories"
        });

        return new CategoriesService(
            factory.Object,
            settings,
            Mock.Of<ILogger<CategoriesService>>());
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

    // Verifies successful category retrieval.
    [Fact]
    public async Task GetCategories_ReturnsCategories()
    {
        var categories = new[]
        {
            new Category { Id = 1, Name = "Category 1" },
            new Category { Id = 2, Name = "Category 2" }
        };

        var service = CreateService(
            CreateHandler(HttpStatusCode.OK, categories).Object);

        var result = await service.GetCategories();

        Assert.Equal(2, result.Count);
        Assert.Equal("Category 1", result.First().Name);
    }

    // Verifies error handling when category retrieval fails.
    [Fact]
    public async Task GetCategories_Throws_WhenRequestFails()
    {
        var service = CreateService(
            CreateHandler(HttpStatusCode.InternalServerError).Object);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.GetCategories());
    }

    // Verifies successful single category retrieval.
    [Fact]
    public async Task GetCategory_ReturnsCategory()
    {
        var category = new Category
        {
            Id = 1,
            Name = "Category 1"
        };

        var service = CreateService(
            CreateHandler(HttpStatusCode.OK, category).Object);

        var result = await service.GetCategory(1);

        Assert.Equal(1, result.Id);
        Assert.Equal("Category 1", result.Name);
    }

    // Verifies error handling when single category retrieval fails.
    [Fact]
    public async Task GetCategory_Throws_WhenRequestFails()
    {
        var service = CreateService(
            CreateHandler(HttpStatusCode.InternalServerError).Object);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.GetCategory(1));
    }

    // Verifies successful category creation.
    [Fact]
    public async Task CreateCategory_ReturnsCategory()
    {
        var request = new CreateCategoryRequest { Name = "New Category" };
        var category = new Category { Id = 1, Name = "New Category" };

        var service = CreateService(
            CreateHandler(HttpStatusCode.OK, category).Object);

        var result = await service.CreateCategory(request);

        Assert.Equal(1, result.Id);
        Assert.Equal("New Category", result.Name);
    }

    // Verifies error handling when category creation fails.
    [Fact]
    public async Task CreateCategory_Throws_WhenRequestFails()
    {
        var request = new CreateCategoryRequest { Name = "New Category" };

        var service = CreateService(
            CreateHandler(HttpStatusCode.InternalServerError).Object);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.CreateCategory(request));
    }

    // Verifies successful category update.
    [Fact]
    public async Task UpdateCategory_ReturnsCategory()
    {
        var request = new UpdateCategoryRequest { Name = "Updated Category" };
        var category = new Category { Id = 1, Name = "Updated Category" };

        var service = CreateService(
            CreateHandler(HttpStatusCode.OK, category).Object);

        var result = await service.UpdateCategory(1, request);

        Assert.Equal(1, result.Id);
        Assert.Equal("Updated Category", result.Name);
    }

    // Verifies error handling when category update fails.
    [Fact]
    public async Task UpdateCategory_Throws_WhenRequestFails()
    {
        var request = new UpdateCategoryRequest { Name = "Updated Category" };

        var service = CreateService(
            CreateHandler(HttpStatusCode.InternalServerError).Object);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.UpdateCategory(1, request));
    }
}