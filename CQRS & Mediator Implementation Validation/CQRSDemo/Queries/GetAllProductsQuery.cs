using CQRSDemo.Models;
using MediatR;

namespace CQRSDemo.Queries;

public class GetAllProductsQuery : IRequest<List<Product>>
{
}
