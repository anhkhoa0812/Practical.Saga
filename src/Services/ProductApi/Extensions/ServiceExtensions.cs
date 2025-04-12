using Confluent.Kafka;
using MassTransit;
using ProductApi.Consumers;
using Shared.Configurations;
using Shared.Events;

namespace ProductApi.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("redis");
        if (string.IsNullOrEmpty(redisConnectionString))
        {
            throw new ArgumentNullException("Redis connection is not configured.");
            
        }
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;

        });
        return services;
    }

    public static IServiceCollection ConfigureKafka(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        var kafkaOptions = configuration
            .GetSection("KafkaOptions")
            .Get<KafkaOptions>();
        services.AddMassTransit(masstransit =>
        {
            masstransit.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
            masstransit.AddRider(rider =>
            {
                rider.AddProducer<string, ChangeProductResponseEvent>(kafkaOptions.Topics
                    .ChangeQuantityProductResponse);
                rider.AddProducer<string, ErrorEvent>(kafkaOptions.Topics.Error);
                rider.AddConsumersFromNamespaceContaining<ChangeQuantityProductRequestConsumer>();
                rider.UsingKafka(kafkaOptions.ClientConfig, (riderContext, kafkaConfig) =>
                {
                    kafkaConfig.TopicEndpoint<string, ChangeQuantityProductRequestEvent>(
                        topicName: kafkaOptions.Topics.ChangeQuantityProductRequest,
                        groupId: kafkaOptions.ConsumerGroup,
                        configure: topicConfig =>
                        {
                            topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                            topicConfig.ConfigureConsumer<ChangeQuantityProductRequestConsumer>(riderContext);
                            topicConfig.DiscardSkippedMessages();
                            topicConfig.CreateIfMissing();
                        });
                });
            });
        });
        return services;
    }
}