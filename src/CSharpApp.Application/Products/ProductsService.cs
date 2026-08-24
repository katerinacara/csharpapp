using System.Net.Http;
using System.Text;
using CSharpApp.Core.Dtos;

namespace CSharpApp.Application.Products;

public class ProductsService : IProductsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<ProductsService> _logger;

    public ProductsService(
        IOptions<RestApiSettings> restApiSettings,
        ILogger<ProductsService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Product>> GetProducts()
    {
        var httpClient = _httpClientFactory.CreateClient("RestApi");

        var response = await httpClient.GetAsync(_restApiSettings.Products);
        
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        
        var res = JsonSerializer.Deserialize<List<Product>>(content);

        return res.AsReadOnly();
    }
    public async Task<Product?> GetProduct(int id)
    {
        var httpClient = _httpClientFactory.CreateClient("RestApi");

        var url = $"{_restApiSettings.Products}/{id}";
        _logger.LogInformation("Getting product from {Url}", url);

        var response = await httpClient.GetAsync($"{_restApiSettings.Products}/{id}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<Product>(content);
    }

    public async Task<Product?> CreateProduct(CreateProductRequest product)
    {
        var httpClient = _httpClientFactory.CreateClient("RestApi");

        var json = JsonSerializer.Serialize(product);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(_restApiSettings.Products, content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<Product>(responseContent);
    }
}