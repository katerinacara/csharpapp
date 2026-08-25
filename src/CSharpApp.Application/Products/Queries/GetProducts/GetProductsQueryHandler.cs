using CSharpApp.Core.Dtos;
using MediatR;

namespace CSharpApp.Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IReadOnlyCollection<Product>>
{
    private readonly IProductsService _productsService;

    public GetProductsQueryHandler(IProductsService productsService)
    {
        _productsService = productsService;
    }

    public async Task<IReadOnlyCollection<Product>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        return await _productsService.GetProducts();
    }
}