using MassTransit;
using OrderApi.Dto;
using OrderApi.GrpcService;
using PaymentApi.Protos;
using Shared.Configurations;
using Shared.Events;

namespace OrderApi.Extensions;

public static class KafkaExtensions
{
    internal static IServiceCollection AddCustomKafka(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        var kafkaOptions = configuration
            .GetSection("KafkaOptions")
            .Get<KafkaOptions>();
        
        services.AddMassTransit(configureMassTransit =>
        {
            configureMassTransit.UsingInMemory();
            configureMassTransit.AddRider(configureRider =>
            {
                configureRider.AddProducer<string, OrderCreatedEvent>(kafkaOptions!.Topics.CreateOrderRequest);
                configureRider.UsingKafka(kafkaOptions.ClientConfig, (_, _) => { });
            });
        });
        
        return services;
    }
    
    public static IServiceCollection ConfigureGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        
        var settings = configuration.GetSection("GrpcSettings")
            .Get<GrpcSettings>();
        if(settings == null || string.IsNullOrEmpty(settings.StockUrl))
            throw new ArgumentNullException("Grpc is not configured.");

        services.AddGrpcClient<PaymentProtoService.PaymentProtoServiceClient>(
            x =>
            {
                x.Address = new Uri(settings.StockUrl);
                x.ChannelOptionsActions.Add(channelOptions =>
                {
                });
            }
            
                
            
        );
        services.AddScoped<PaymentGrpcService>();

        return services;
    }
}