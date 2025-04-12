using MassTransit;
using Shared.Events;

namespace Orchestrator;

public class ChangQuantityProductActivity : IStateMachineActivity<OrderSagaState, OrderCreatedEvent>
{
    private readonly ITopicProducer<string, ChangeQuantityProductRequestEvent> _producer;
    public ChangQuantityProductActivity(ITopicProducer<string, ChangeQuantityProductRequestEvent> producer)
    {
        _producer = producer;
    }
    public void Probe(ProbeContext context)
    {
        context.CreateScope("ChangeQuantityProductActivity");
    }
    public async Task Execute(BehaviorContext<OrderSagaState, OrderCreatedEvent> context, IBehavior<OrderSagaState, OrderCreatedEvent> next)
    {
        var products = context.Message.Products
            .Select(x => new ProductDto { Id = x.Id, Quantity = x.Quantity })
            .ToList();

        var changeQuantityEvent = new ChangeQuantityProductRequestEvent
        {
            Products = products,
        };

        await _producer.Produce(
            context.CorrelationId.ToString(),
            changeQuantityEvent,
            context.CancellationToken);

        Console.WriteLine($"[Saga] Published ChangeQuantityProductRequestEvent for {context.CorrelationId!.Value}");

        await next.Execute(context).ConfigureAwait(false);
    }

    public async Task Faulted<TException>(BehaviorExceptionContext<OrderSagaState, OrderCreatedEvent, TException> context, IBehavior<OrderSagaState, OrderCreatedEvent> next) where TException : Exception
    {
        await next.Faulted(context).ConfigureAwait(false);
    }

    void IVisitable.Accept(StateMachineVisitor visitor) => visitor.Visit(this);
}