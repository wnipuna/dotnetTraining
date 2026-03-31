using CQRSDemo.Commands;
using CQRSDemo.Data;
using CQRSDemo.Models;
using MediatR;

namespace CQRSDemo.Handlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
{
    private readonly InMemoryDataStore _dataStore;

    public CreateProductCommandHandler(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock
        };

        var createdProduct = _dataStore.AddProduct(product);
        return Task.FromResult(createdProduct);
    }
}
