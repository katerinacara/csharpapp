using CSharpApp.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

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

app.Run();