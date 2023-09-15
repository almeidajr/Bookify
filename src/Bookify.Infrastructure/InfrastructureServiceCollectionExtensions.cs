using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Reviews;
using Bookify.Domain.Users;
using Bookify.Infrastructure.Authentication;
using Bookify.Infrastructure.Clock;
using Bookify.Infrastructure.Data;
using Bookify.Infrastructure.Email;
using Bookify.Infrastructure.Repositories;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddSingleton<IDateTimeProvider, DateTimeProvider>()
            .AddSingleton<IEmailService, EmailService>()
            .AddPersistence(configuration)
            .Authentication(configuration);
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

        var connectionString = configuration.GetConnectionString("Database") ??
                               throw new InvalidOperationException(
                                   "Database connection string not found in configuration.");

        return services.AddSingleton<ISqlConnectionFactory>(new SqlConnectionFactory(connectionString))
            .AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseNpgsql(configuration.GetConnectionString("Database"))
                    .UseSnakeCaseNamingConvention())
            .AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>())
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IApartmentRepository, ApartmentRepository>()
            .AddScoped<IBookingRepository, BookingRepository>()
            .AddScoped<IReviewRepository, ReviewRepository>();
    }

    private static IServiceCollection Authentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<AuthenticationOptions>(configuration.GetSection(AuthenticationOptions.Section))
            .Configure<KeycloakOptions>(configuration.GetSection(KeycloakOptions.Section))
            .ConfigureOptions<JwtBearerOptionsSetup>()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services
            .AddTransient<AdminAuthorizationDelegatingHandler>()
            .AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpClient) =>
            {
                var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;
                httpClient.BaseAddress = keycloakOptions.AdminUrl;
            })
            .AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

        return services;
    }
}