using CSharpApp.Application.Products;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace CSharpApp.Tests.Products;

public class ProductServiceTests
{
    private static ProductsService CreateService(HttpMessageHandler handler)
    {
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.com")
        };

        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(x => x.CreateClient("RestApi")).Returns(client);

        var settings = Options.Create(new RestApiSettings
        {
            Products = "/api/products"
        });

        return new ProductsService(
            settings,
            Mock.Of<ILogger<ProductsService>>(),
            factory.Object);
    }

    private static Mock<HttpMessageHandler> CreateHandler(
        HttpStatusCode status,
        object? content = null)
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


    // Verifies successful product retrieval.

    [Fact]
    public async Task GetProducts_ReturnsProducts()
    {
        var products = new[]
        {
            new Product { Id = 1, Title = "Product 1" },
            new Product { Id = 2, Title = "Product 2" }
        };

        var service = CreateService(
            CreateHandler(HttpStatusCode.OK, products).Object);

        var result = await service.GetProducts();

        Assert.Equal(2, result.Count);
        Assert.Equal("Product 1", result.First().Title);
    }

    // Verifies error handling when product retrieval fails.

    [Fact]
    public async Task GetProducts_Throws_WhenRequestFails()
    {
        var service = CreateService(
            CreateHandler(HttpStatusCode.InternalServerError).Object);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.GetProducts());
    }

    // Verifies successful retrieval of a single product.

    [Fact]
    public async Task GetProduct_ReturnsProduct()
    {
        var product = new Product
        {
            Id = 1,
            Title = "Test Product"
        };

        var service = CreateService(
            CreateHandler(HttpStatusCode.OK, product).Object);

        var result = await service.GetProduct(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Product", result.Title);
    }

    // Verifies error handling when retrieving a single product fails.

    [Fact]
    public async Task GetProduct_Throws_WhenRequestFails()
    {
        var service = CreateService(
            CreateHandler(HttpStatusCode.InternalServerError).Object);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.GetProduct(1));
    }

    // Verifies successful product creation.

    [Fact]
    public async Task CreateProduct_ReturnsProduct()
    {
        var request = new CreateProductRequest
        {
            Title = "New Product"
        };

        var product = new Product
        {
            Id = 1,
            Title = "New Product"
        };

        var service = CreateService(
            CreateHandler(HttpStatusCode.OK, product).Object);

        var result = await service.CreateProduct(request);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("New Product", result.Title);
    }

    // Verifies error handling when product creation fails.

    [Fact]
    public async Task CreateProduct_Throws_WhenRequestFails()
    {
        var request = new CreateProductRequest
        {
            Title = "New Product"
        };

        var service = CreateService(
            CreateHandler(HttpStatusCode.InternalServerError).Object);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.CreateProduct(request));
    }

    // Verifies that product creation sends a POST request to the correct endpoint.

    [Fact]
    public async Task CreateProduct_SendsPostToProductsEndpoint()
    {
        var request = new CreateProductRequest
        {
            Title = "New Product"
        };

        var handler = CreateHandler(HttpStatusCode.OK, new Product
        {
            Id = 1,
            Title = "New Product"
        });

        var service = CreateService(handler.Object);

        await service.CreateProduct(request);

        handler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(r =>
                r.Method == HttpMethod.Post &&
                r.RequestUri!.PathAndQuery == "/api/products"),
            ItExpr.IsAny<CancellationToken>());
    }
}