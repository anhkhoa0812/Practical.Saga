using Confluent.Kafka;
using MassTransit;
using Shared.Configurations;
using Shared.Events;

namespace Orchestrator.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddCustomKafka(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        var kafkaOptions = configuration
            .GetSection(nameof(KafkaOptions))
            .Get<KafkaOptions>();

        services.AddMassTransit(massTransit =>
        {
            // massTransit.AddSagaStateMachine<OrderStateMachine, OrderSagaState>().InMemoryRepository();
            massTransit.UsingInMemory((context, cfg) =>  cfg.ConfigureEndpoints(context));
            massTransit.AddRider(rider =>
            {
                rider
                    .AddSagaStateMachine<OrderStateMachine, OrderSagaState>().InMemoryRepository();

                rider.AddProducer<string, ChangeQuantityProductRequestEvent>(kafkaOptions!.Topics.ChangeQuantityProductRequest);
                
                rider.UsingKafka(kafkaOptions.ClientConfig, (riderContext, kafkaConfig) =>
                {
                    kafkaConfig.TopicEndpoint<string, OrderCreatedEvent>(
                        topicName: kafkaOptions!.Topics.CreateOrderRequest,
                        groupId: kafkaOptions.ConsumerGroup,
                        configure: topicConfig =>
                        {
                            topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                            topicConfig.ConfigureSaga<OrderSagaState>(riderContext);
                            topicConfig.DiscardSkippedMessages();
                            topicConfig.UseInMemoryOutbox(riderContext);
                            topicConfig.CreateIfMissing();
                        });
                    kafkaConfig.TopicEndpoint<string, ChangeProductResponseEvent>(
                       topicName: kafkaOptions.Topics.ChangeQuantityProductResponse,
                       groupId: kafkaOptions.ConsumerGroup,
                       configure: topicConfig =>
                       {
                           topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                           topicConfig.DiscardSkippedMessages();
                           topicConfig.ConfigureSaga<OrderSagaState>(riderContext);
                           topicConfig.UseInMemoryOutbox(riderContext);
                           topicConfig.CreateIfMissing();
                       });

                    kafkaConfig.TopicEndpoint<string, ErrorEvent>(
                       topicName: kafkaOptions.Topics.Error,
                       groupId: kafkaOptions.ConsumerGroup,
                       configure: topicConfig =>
                       {
                           topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                           topicConfig.DiscardSkippedMessages();
                           topicConfig.ConfigureSaga<OrderSagaState>(riderContext);
                           topicConfig.UseInMemoryOutbox(riderContext);
                           topicConfig.CreateIfMissing();
                       });
                });
            });
        });
        
        return services;
    }
}