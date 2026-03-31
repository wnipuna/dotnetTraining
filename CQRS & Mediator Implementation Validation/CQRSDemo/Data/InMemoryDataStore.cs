using CQRSDemo.Models;

namespace CQRSDemo.Data;

public class InMemoryDataStore
{
    private readonly List<Product> _products = new();
    private int _nextId = 1;

    public InMemoryDataStore()
    {
        _products.Add(new Product
        {
            Id = _nextId++,
            Name = "Laptop",
            Description = "High-performance laptop",
            Price = 1200.00m,
            Stock = 10,
            CreatedAt = DateTime.UtcNow
        });
        
        _products.Add(new Product
        {
            Id = _nextId++,
            Name = "Mouse",
            Description = "Wireless mouse",
            Price = 25.00m,
            Stock = 50,
            CreatedAt = DateTime.UtcNow
        });
    }

    public List<Product> GetAllProducts() => _products;

    public Product? GetProductById(int id) => _products.FirstOrDefault(p => p.Id == id);

    public Product AddProduct(Product product)
    {
        product.Id = _nextId++;
        product.CreatedAt = DateTime.UtcNow;
        _products.Add(product);
        return product;
    }

    public bool UpdateProduct(Product product)
    {
        var existingProduct = GetProductById(product.Id);
        if (existingProduct == null) return false;

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.Stock = product.Stock;
        return true;
    }

    public bool DeleteProduct(int id)
    {
        var product = GetProductById(id);
        if (product == null) return false;
        
        _products.Remove(product);
        return true;
    }
}
