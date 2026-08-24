using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Application.Categories;

public class CategoriesService : ICategoriesService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<CategoriesService> _logger;

    public CategoriesService(
        IHttpClientFactory httpClientFactory,
        IOptions<RestApiSettings> restApiSettings,
        ILogger<CategoriesService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Category>> GetCategories()
    {

        var httpClient = _httpClientFactory.CreateClient("RestApi");

        var response = await httpClient.GetAsync(_restApiSettings.Categories);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<List<Category>>(content);

        if (result is null)
        {
            return Array.Empty<Category>();
        }

        return result.AsReadOnly();
    }

    public async Task<Category> GetCategory(int id)
    {
        var httpClient = _httpClientFactory.CreateClient("RestApi");

        var response = await httpClient.GetAsync(
            $"{_restApiSettings.Categories}/{id}");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<Category>(content)
            ?? throw new InvalidOperationException(
                "Category could not be deserialized.");
    }

    public async Task<Category> CreateCategory(CreateCategoryRequest request)
    {
        var httpClient = _httpClientFactory.CreateClient("RestApi");

        var response = await httpClient.PostAsJsonAsync(
            _restApiSettings.Categories,
            request);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<Category>(content)
            ?? throw new InvalidOperationException(
                "Category could not be deserialized.");
    }

    public async Task<Category> UpdateCategory(
        int id,
        UpdateCategoryRequest request)
    {
        var httpClient = _httpClientFactory.CreateClient("RestApi");

        var response = await httpClient.PutAsJsonAsync(
            $"{_restApiSettings.Categories}/{id}",
            request);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<Category>(content)
            ?? throw new InvalidOperationException(
                "Category could not be deserialized.");
    }
}