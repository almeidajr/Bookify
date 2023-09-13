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
}