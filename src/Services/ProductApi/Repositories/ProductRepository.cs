using Microsoft.Extensions.Caching.Distributed;
using ProductApi.Models;

namespace ProductApi.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IDistributedCache _redisCacheService;
    
    public ProductRepository(IDistributedCache redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }
    public async Task<bool> UpdateQuantity(Guid productId, int quantity)
    {
        var product = await _redisCacheService.GetStringAsync(productId.ToString());
        if (string.IsNullOrEmpty(product))
        {
            return false;
        }
        var productData = System.Text.Json.JsonSerializer.Deserialize<Product>(product);
        productData.Quantity = quantity;
        var productDataString = System.Text.Json.JsonSerializer.Serialize(productData);
        await _redisCacheService.SetStringAsync(productId.ToString(), productDataString);
        return true;
    }
    public async Task<Product> GetProduct(Guid id)
    {
        var product = await _redisCacheService.GetStringAsync(id.ToString());
        if (string.IsNullOrEmpty(product))
        {
            return null;
        }
        return System.Text.Json.JsonSerializer.Deserialize<Product>(product);
    }

    public async Task<Product> CreateProduct(Product product)
    {
        var productData = System.Text.Json.JsonSerializer.Serialize(product);
        await _redisCacheService.SetStringAsync(product.Id.ToString(), productData);
        return product;
    }
}