using Asp.Versioning.Builder;
using CSharpApp.Application.Products.Commands.CreateProduct;
using CSharpApp.Application.Products.Queries.GetProduct;
using CSharpApp.Application.Products.Queries.GetProducts;
using CSharpApp.Core.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CSharpApp.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IVersionedEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("api/v{version:apiVersion}/products",
            async ([FromServices] IMediator mediator) =>
            {
                var products = await mediator.Send(new GetProductsQuery());
                return Results.Ok(products);
            })
            .WithName("GetProducts")
            .WithSummary("Get all products")
            .HasApiVersion(1.0);

        endpoints.MapGet("api/v{version:apiVersion}/products/{id:int}",
            async (int id, [FromServices] IMediator mediator) =>
            {
                var product = await mediator.Send(new GetProductQuery(id));
                return product is null ? Results.NotFound() : Results.Ok(product);
            })
            .WithName("GetProduct")
            .WithSummary("Get a product by ID")
            .HasApiVersion(1.0);

        endpoints.MapPost("api/v{version:apiVersion}/products",
            async (
                CreateProductRequest product,
                [FromServices] IMediator mediator) =>
            {
                var createdProduct = await mediator.Send(
                    new CreateProductCommand(product));

                return Results.Ok(createdProduct);
            })
            .WithName("CreateProduct")
            .WithSummary("Create a product")
            .HasApiVersion(1.0);
    }
}