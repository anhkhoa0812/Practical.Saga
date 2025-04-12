using MassTransit;
using Shared.Events;

namespace Orchestrator;

public sealed class OrderStateMachine : MassTransitStateMachine<OrderSagaState>
{
    private readonly IServiceProvider _serviceProvider;
    public OrderStateMachine(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InstanceState(x => x.CurrentState);
        
        Event(() => ChangeProductResponseEvent, x =>
        {
            x.CorrelateById(ctx => ctx.CorrelationId ?? Guid.NewGuid());
            x.SelectId(ctx => ctx.CorrelationId ?? Guid.NewGuid());
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => ErrorEvent, x =>
        {
            x.CorrelateById(ctx => ctx.CorrelationId ?? Guid.NewGuid());
            x.SelectId(ctx => ctx.CorrelationId ?? Guid.NewGuid());
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => OrderCreatedEvent, x =>
        {
            x.CorrelateById(ctx => ctx.CorrelationId ?? Guid.NewGuid());
            x.SelectId(ctx => ctx.CorrelationId ?? Guid.NewGuid());
            
        });
        Initially(
            When(OrderCreatedEvent)
                .Then(context =>
                {
                    Console.WriteLine($"[Saga] Received OrderCreatedEvent for {context.CorrelationId!.Value}");
                })
                .Activity(config => config.OfType<ChangQuantityProductActivity>())
                .TransitionTo(OrderCreated)
            
        );
        During(OrderCreated,
            When(ChangeProductResponseEvent) 
                .Then(context =>
                {
                    Console.WriteLine($"[Saga] Received ChangeProductResponseEvent for {context.CorrelationId!.Value}");
                })
                .TransitionTo(ChangeProductQuantitySuccess)
                .Finalize(), 
            When(ErrorEvent)
                .Then(context =>
                {
                    Console.WriteLine($"[Saga] Received ErrorEvent for {context.CorrelationId!.Value}");
                })
                .TransitionTo(ChangeProductQuantityFailed)
                .Finalize()
        );
        SetCompletedWhenFinalized();
    }
    
    public Event<ChangeProductResponseEvent> ChangeProductResponseEvent { get; private set; }
    public Event<ErrorEvent> ErrorEvent { get; private set; }
    public Event<OrderCreatedEvent> OrderCreatedEvent { get; private set; }
    
    public State OrderCreated { get; private set; }
    public State ChangeProductQuantitySuccess { get; private set; }
    public State ChangeProductQuantityFailed { get; private set; }
    
}