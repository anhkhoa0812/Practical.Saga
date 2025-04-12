using Confluent.Kafka;
using MassTransit;
using ProductApi.Repositories;
using Shared.Configurations;
using Shared.Events;

namespace ProductApi.Consumers;

public class ChangeQuantityProductRequestConsumer : IConsumer<ChangeQuantityProductRequestEvent>
{
    private readonly IProductRepository _productRepository;
    private readonly ITopicProducer<string, ChangeProductResponseEvent> _producer;
    private readonly ITopicProducer<string, ErrorEvent> _errorProducer;
    public ChangeQuantityProductRequestConsumer(IProductRepository productRepository,
        ITopicProducer<string, ChangeProductResponseEvent> producer, ITopicProducer<string, ErrorEvent> errorProducer)
    {
        _productRepository = productRepository;
        _producer = producer;
        _errorProducer = errorProducer;
    }

    public async Task Consume(ConsumeContext<ChangeQuantityProductRequestEvent> context)
    {
        var key = Guid.NewGuid();
        var products = context.Message.Products;
        foreach (var product in products)
        {
            var redisProduct = await _productRepository.GetProduct(product.Id);
            if (redisProduct == null)
                await _errorProducer.Produce(
                    key.ToString(),
                    new ErrorEvent()
                    {
                        Error = "Product not found",
                    },
                    context.CancellationToken
                ).ConfigureAwait(false);
            else
            {
                await _productRepository.UpdateQuantity(product.Id, redisProduct.Quantity - product.Quantity);
                await _producer.Produce(
                    key.ToString(),
                    new ChangeProductResponseEvent()
                    {
                        Status = "Success"
                    },
                    context.CancellationToken
                ).ConfigureAwait(false);
                Console.WriteLine($"[ProductApi] Product {product.Id} quantity changed to {redisProduct.Quantity - product.Quantity}");
            }
        }
    }
}
