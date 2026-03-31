using CQRSDemo.Commands;
using CQRSDemo.Data;
using CQRSDemo.Models;
using MediatR;

namespace CQRSDemo.Handlers;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly InMemoryDataStore _dataStore;

    public UpdateProductCommandHandler(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock
        };

        var result = _dataStore.UpdateProduct(product);
        return Task.FromResult(result);
    }
}
