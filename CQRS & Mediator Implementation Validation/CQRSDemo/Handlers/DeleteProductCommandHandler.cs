using CQRSDemo.Commands;
using CQRSDemo.Data;
using MediatR;

namespace CQRSDemo.Handlers;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly InMemoryDataStore _dataStore;

    public DeleteProductCommandHandler(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var result = _dataStore.DeleteProduct(request.Id);
        return Task.FromResult(result);
    }
}
