using MediatR;
using Shared.Events;

namespace OrderApi.Commands;

public class CreateOrderCommand : IRequest<string>
{
    public List<ProductDto> Products { get; set; }
    public long TotalPrice { get; set; }
}