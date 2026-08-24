using System.Text;
using CSharpApp.Core.Dtos;

namespace CSharpApp.Application.Products;

public class ProductsService : IProductsService
{
    private readonly HttpClient _httpClient;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<ProductsService> _logger;

    public ProductsService(IOptions<RestApiSettings> restApiSettings,
        ILogger<ProductsService> logger, HttpClient httpClient)
    {
        _httpClient = new HttpClient();
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Product>> GetProducts()
    {
        _httpClient.BaseAddress = new Uri(_restApiSettings.BaseUrl!);
        var response = await _httpClient.GetAsync(_restApiSettings.Products);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<List<Product>>(content);

        return res.AsReadOnly();
    }

    public async Task<Product?> GetProduct(int id)
    {
        _httpClient.BaseAddress = new Uri(_restApiSettings.BaseUrl!);

        var url = $"{_restApiSettings.Products}/{id}";
        _logger.LogInformation("Getting product from {Url}", url);

        var response = await _httpClient.GetAsync($"{_restApiSettings.Products}/{id}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<Product>(content);
    }

    public async Task<Product?> CreateProduct(CreateProductRequest product)
    {
        _httpClient.BaseAddress = new Uri(_restApiSettings.BaseUrl!);

        var json = JsonSerializer.Serialize(product);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_restApiSettings.Products, content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<Product>(responseContent);
    }
}