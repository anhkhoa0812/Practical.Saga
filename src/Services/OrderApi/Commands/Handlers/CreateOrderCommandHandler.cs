using MassTransit;
using MediatR;
using OrderApi.Dto;
using OrderApi.Entities;
using OrderApi.GrpcService;
using OrderApi.Repositories;
using Shared.Events;

namespace OrderApi.Commands.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, string>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ITopicProducer<string, OrderCreatedEvent> _producer;
    private readonly PaymentGrpcService _paymentGrpcService;
    public CreateOrderCommandHandler(IOrderRepository orderRepository, ITopicProducer<string, OrderCreatedEvent> producer, PaymentGrpcService paymentGrpcService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _producer = producer ?? throw new ArgumentNullException(nameof(producer));  
        _paymentGrpcService = paymentGrpcService ?? throw new ArgumentNullException(nameof(paymentGrpcService));
    }
    public async Task<string> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
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
        var paymentUrl = await _paymentGrpcService.CreatePayment(order.Id.ToString(), request.TotalPrice, Guid.NewGuid().ToString());
        await _producer.Produce(
                key: order.Id.ToString(),
                orderCreatedEvent,
                Pipe.Execute<KafkaSendContext>(p =>
                {
                    p.CorrelationId = correlationId;
                }), cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        Console.WriteLine($"[OrderApi] Order created with ID: {order.Id}");
        return paymentUrl;
    }
}