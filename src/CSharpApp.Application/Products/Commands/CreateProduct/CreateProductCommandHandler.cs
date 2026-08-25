using CSharpApp.Core.Dtos;
using MediatR;

namespace CSharpApp.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product?>
{
    private readonly IProductsService _productsService;

    public CreateProductCommandHandler(IProductsService productsService)
    {
        _productsService = productsService;
    }

    public async Task<Product?> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        return await _productsService.CreateProduct(request.Product);
    }
}