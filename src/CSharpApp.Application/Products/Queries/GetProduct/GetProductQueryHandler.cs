using CSharpApp.Core.Dtos;
using MediatR;

namespace CSharpApp.Application.Products.Queries.GetProduct;

public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Product?>
{
    private readonly IProductsService _productsService;

    public GetProductQueryHandler(IProductsService productsService)
    {
        _productsService = productsService;
    }

    public async Task<Product?> Handle(
        GetProductQuery request,
        CancellationToken cancellationToken)
    {
        return await _productsService.GetProduct(request.Id);
    }
}