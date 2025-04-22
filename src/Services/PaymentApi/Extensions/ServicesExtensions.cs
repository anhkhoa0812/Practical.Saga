using Shared.Configurations;

namespace PaymentApi.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var momoSettings = configuration.GetSection(nameof(MomoOptions))
            .Get<MomoOptions>();
        services.AddSingleton(momoSettings);
        return services;
    }
}