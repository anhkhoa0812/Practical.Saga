using Confluent.Kafka;

namespace Shared.Configurations;

public sealed record KafkaOptions
{
    public string ConsumerGroup { get; set; } = default!;
    public Topics Topics { get; set; } = default!;
    public ClientConfig ClientConfig { get; set; } = default!;
    // public MongoDb MongoDb { get; set; } = default!;
    // public RedisDb RedisDb { get; set; } = default!;
};
public sealed record Topics
{
    public string CreateOrderRequest { get; set; } = default!;
    public string CreateOrderResponse { get; set; } = default!;
    public string ChangeQuantityProductRequest { get; set; } = default!;
    public string ChangeQuantityProductResponse { get; set; } = default!;
    public string Error { get; set; } = default!;
}