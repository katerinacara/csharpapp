using Asp.Versioning.Builder;
using CSharpApp.Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace CSharpApp.Api.Endpoints
{
    public static class CategoryEndpoints
    {
        public static void MapCategoryEndpoints(this IVersionedEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("api/v{version:apiVersion}/getcategories",
                async ([FromServices] ICategoriesService categoriesService)
                    => await categoriesService.GetCategories())
                .WithName("GetCategories")
                .WithSummary("Get all categories")
                .HasApiVersion(1.0);

            endpoints.MapGet("api/v{version:apiVersion}/getcategories/{id:int}",
                async (int id, [FromServices] ICategoriesService categoriesService)
                   => await categoriesService.GetCategory(id))
                .WithName("GetCategory")
                .WithSummary("Get a caregory by ID")
                .HasApiVersion(1.0);

            endpoints.MapPost("api/v{version:apiVersion}/createcategory",
                async (CreateCategoryRequest request, [FromServices] ICategoriesService categoriesService) =>
                {
                    var category = await categoriesService.CreateCategory(request);
                    return category is null ? Results.NotFound() : Results.Ok(category);
                })
                .WithName("CreateCategory")
                .WithSummary("Create a category")
                .HasApiVersion(1.0);

            endpoints.MapPut("api/v{version:apiVersion}/updatecategory/{id:int}",
                async (int id, UpdateCategoryRequest request,[FromServices] ICategoriesService categoriesService) =>
                {
                    var category = await categoriesService.UpdateCategory(id, request);
                    return Results.Ok(category);
                })
                .WithName("UpdateCategory")
                .WithSummary("Update a category")
                .HasApiVersion(1.0);

        }
    }
}
