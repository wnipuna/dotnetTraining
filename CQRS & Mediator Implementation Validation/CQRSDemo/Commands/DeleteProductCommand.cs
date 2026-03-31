using MediatR;

namespace CQRSDemo.Commands;

public class DeleteProductCommand : IRequest<bool>
{
    public int Id { get; set; }
}
