using Bogus;
using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;
using Bookify.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Api.Extensions;

public static class ApiBuilderExtensions
{
    public static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public static async Task SeedDataAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var faker = new Faker();

        if (!await dbContext.Set<Apartment>().AnyAsync())
        {
            var amenities = Enum.GetValues<Amenity>();

            var apartments = Enumerable.Range(0, 100)
                .Select(_ => new Apartment(
                    Guid.NewGuid(),
                    new Name(faker.Company.CompanyName()),
                    new Description(faker.Company.CatchPhrase()),
                    new Address(
                        faker.Address.Country(),
                        faker.Address.State(),
                        faker.Address.ZipCode(),
                        faker.Address.City(),
                        faker.Address.StreetAddress()),
                    new Money(faker.Finance.Amount(50, 1_500), Currency.Usd),
                    new Money(faker.Finance.Amount(25, 250), Currency.Usd),
                    faker.PickRandom(amenities, faker.Random.Int(0, amenities.Length)).ToList()));
            dbContext.AddRange(apartments);
        }

        await dbContext.SaveChangesAsync();
    }
}