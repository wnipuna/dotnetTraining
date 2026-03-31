using CQRSDemo.Models;
using MediatR;

namespace CQRSDemo.Queries;

public class GetProductByIdQuery : IRequest<Product?>
{
    public int Id { get; set; }
}
