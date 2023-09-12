﻿using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Reviews;
using Bookify.Domain.Users;
using Bookify.Infrastructure.Clock;
using Bookify.Infrastructure.Email;
using Bookify.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddSingleton<IDateTimeProvider, DateTimeProvider>()
            .AddSingleton<IEmailService, EmailService>()
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
}