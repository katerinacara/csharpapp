using CSharpApp.Core.Dtos;
using MediatR;

namespace CSharpApp.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(CreateProductRequest Product) : IRequest<Product?>;