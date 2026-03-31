using CQRSDemo.Models;
using MediatR;

namespace CQRSDemo.Commands;

public class CreateProductCommand : IRequest<Product>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
