using OrderApi.Dto;
using OrderApi.Entities;

namespace OrderApi.Repositories;

public interface IOrderRepository
{
    Task<Order> CreateOrder(Order order); 
}