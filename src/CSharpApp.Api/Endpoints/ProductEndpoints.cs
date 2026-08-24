using Asp.Versioning.Builder;
using CSharpApp.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CSharpApp.Api.Endpoints
{
    public static class ProductEndpoints
    {
        public static void MapProductEndpoints(this IVersionedEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("api/v{version:apiVersion}/getproducts",
                async ([FromServices] IProductsService productsService) =>
                {
                    var products = await productsService.GetProducts();
                    return Results.Ok(products);
                })
                .WithName("GetProducts")
                .WithSummary("Get all products")
                .HasApiVersion(1.0);

            endpoints.MapGet("api/v{version:apiVersion}/getproduct/{id:int}",
                async (int id, [FromServices] IProductsService productsService) =>
                {
                    var product = await productsService.GetProduct(id);
                    return product is null ? Results.NotFound() : Results.Ok(product);
                })
                .WithName("GetProduct")
                .WithSummary("Get a product by ID")
                .HasApiVersion(1.0);

            endpoints.MapPost("api/v{version:apiVersion}/createproduct",
                async (
                    CreateProductRequest product,
                    [FromServices] IProductsService productsService) =>
                {
                    var createdProduct = await productsService.CreateProduct(product);
                    return Results.Ok(createdProduct);
                })
                .WithName("CreateProduct")
                .WithSummary("Create a product")
                .HasApiVersion(1.0);
        }
    }
}
