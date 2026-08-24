using CSharpApp.Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Logging.ClearProviders().AddSerilog(logger);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDefaultConfiguration();
builder.Services.AddHttpConfiguration(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.AddApiVersioning();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


var versionedEndpointRouteBuilder = app.NewVersionedApi();

versionedEndpointRouteBuilder.MapGet(
    "api/v{version:apiVersion}/getproducts",
    async ([FromServices] IProductsService productsService) =>
    {
        var products = await productsService.GetProducts();
        return products;
    })
    .WithName("GetProducts")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapGet(
    "api/v{version:apiVersion}/getproduct/{id:int}",
    async (int id, [FromServices] IProductsService productsService) =>
    {
        var product = await productsService.GetProduct(id);
        return product;
    })
    .WithName("GetProduct")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapPost(
    "api/v{version:apiVersion}/createproduct",
    async (
        CreateProductRequest product,
        [FromServices] IProductsService productsService) =>
    {
        var createdProduct = await productsService.CreateProduct(product);
        return Results.Ok(createdProduct);
    })
    .WithName("CreateProduct")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapGet(
    "api/v{version:apiVersion}/getcategories",
    async ([FromServices] ICategoriesService categoriesService) =>
    {
        var categories = await categoriesService.GetCategories();
        return categories;
    })
    .WithName("GetCategories")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapGet(
    "api/v{version:apiVersion}/getcategories/{id:int}",
    async (int id, [FromServices] ICategoriesService categoriesService) =>
    {
        var category = await categoriesService.GetCategory(id);
        return category;
    })
    .WithName("GetCategory")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapPost(
    "api/v{version:apiVersion}/createcategory",
    async (
        CreateCategoryRequest request,
        [FromServices] ICategoriesService categoriesService) =>
    {
        var category = await categoriesService.CreateCategory(request);
        return Results.Ok(category);
    })
    .WithName("CreateCategory")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapPut(
    "api/v{version:apiVersion}/updatecategory/{id:int}",
    async (
        int id,
        UpdateCategoryRequest request,
        [FromServices] ICategoriesService categoriesService) =>
    {
        var category = await categoriesService.UpdateCategory(id, request);
        return Results.Ok(category);
    })
    .WithName("UpdateCategory")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapPost(
    "api/v{version:apiVersion}/auth/login",
    async ([FromServices] IAuthService authService) =>
    {
        var response = await authService.Login();

        return Results.Ok(response);
    })
    .WithName("Login")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapGet(
    "api/v{version:apiVersion}/auth/profile",
    async ([FromServices] IAuthService authService) =>
    {
        var profile = await authService.GetProfile();

        return Results.Ok(profile);
    })
    .WithName("GetProfile")
    .HasApiVersion(1.0);

app.Run();