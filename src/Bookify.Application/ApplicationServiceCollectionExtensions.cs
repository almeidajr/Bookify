using Bookify.Application.Abstractions.Behaviors;
using Bookify.Domain.Bookings;
using FluentValidation;
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
                    .AddOpenBehavior(typeof(LoggingBehavior<,>))
                    .AddOpenBehavior(typeof(ValidationBehavior<,>)))
            .AddValidatorsFromAssembly(typeof(ApplicationServiceCollectionExtensions).Assembly)
            .AddSingleton<PricingService>();
    }
}