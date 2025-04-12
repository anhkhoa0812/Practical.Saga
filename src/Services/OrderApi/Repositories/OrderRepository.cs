using OrderApi.Entities;

namespace OrderApi.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _dbContext;
    public OrderRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<Order> CreateOrder(Order order)
    {
        _dbContext.Orders.Add(order);
        _dbContext.SaveChanges();
        return Task.FromResult(order);
    }
}