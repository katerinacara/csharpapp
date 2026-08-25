using CSharpApp.Core.Dtos;
using MediatR;

namespace CSharpApp.Application.Products.Queries.GetProducts;

public record GetProductsQuery : IRequest<IReadOnlyCollection<Product>>;