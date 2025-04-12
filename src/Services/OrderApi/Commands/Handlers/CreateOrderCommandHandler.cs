using MassTransit;
using MediatR;
using OrderApi.Dto;
using OrderApi.Entities;
using OrderApi.Repositories;
using Shared.Events;

namespace OrderApi.Commands.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ITopicProducer<string, OrderCreatedEvent> _producer;
    public CreateOrderCommandHandler(IOrderRepository orderRepository, ITopicProducer<string, OrderCreatedEvent> producer)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _producer = producer ?? throw new ArgumentNullException(nameof(producer));  
    }
    public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order()
        {
            Id = Guid.NewGuid(),
            ProductIds = request.Products.Select(x => x.Id).ToList(),
            TotalPrice = request.TotalPrice,
        };
        await _orderRepository.CreateOrder(order);
        var orderCreatedEvent = new OrderCreatedEvent()
        {
            Products = request.Products
        };
        var correlationId = Guid.NewGuid();
        await _producer.Produce(
                key: order.Id.ToString(),
                orderCreatedEvent,
                Pipe.Execute<KafkaSendContext>(p =>
                {
                    p.CorrelationId = correlationId;
                }), cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        Console.WriteLine($"[OrderApi] Order created with ID: {order.Id}");
    }
}