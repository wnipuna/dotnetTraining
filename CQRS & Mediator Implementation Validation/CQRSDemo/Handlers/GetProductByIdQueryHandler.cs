using CQRSDemo.Data;
using CQRSDemo.Models;
using CQRSDemo.Queries;
using MediatR;

namespace CQRSDemo.Handlers;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product?>
{
    private readonly InMemoryDataStore _dataStore;

    public GetProductByIdQueryHandler(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = _dataStore.GetProductById(request.Id);
        return Task.FromResult(product);
    }
}
