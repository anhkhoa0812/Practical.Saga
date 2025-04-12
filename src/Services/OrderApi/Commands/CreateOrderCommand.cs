using MediatR;
using Shared.Events;

namespace OrderApi.Commands;

public class CreateOrderCommand : IRequest
{
    public List<ProductDto> Products { get; set; }
    public decimal TotalPrice { get; set; }
}