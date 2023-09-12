using Bookify.Application.Abstractions.Behaviors;
using Bookify.Domain.Bookings;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddMediatR(configuration =>
                configuration
                    .RegisterServicesFromAssembly(typeof(ApplicationServiceCollectionExtensions).Assembly)
                    .AddOpenBehavior(typeof(LoggingBehavior<,>)))
            .AddSingleton<PricingService>();
    }
}