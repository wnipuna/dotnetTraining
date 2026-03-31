using CQRSDemo.Data;
using CQRSDemo.Models;
using CQRSDemo.Queries;
using MediatR;

namespace CQRSDemo.Handlers;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<Product>>
{
    private readonly InMemoryDataStore _dataStore;

    public GetAllProductsQueryHandler(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<List<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = _dataStore.GetAllProducts();
        return Task.FromResult(products);
    }
}
