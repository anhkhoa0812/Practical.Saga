using ProductApi.Models;

namespace ProductApi.Repositories;

public interface IProductRepository
{
    Task<bool> UpdateQuantity(Guid productId, int quantity);
    Task<Product> GetProduct(Guid id);
    Task<Product> CreateProduct(Product product);
}