using System.Text.Json;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public ApplicationDbContext(DbContextOptions options, IDateTimeProvider dateTimeProvider) : base(options)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        try
        {
            AddDomainEventsAsOutboxMessages();
            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException(
                "A concurrency conflict occurred while trying to save changes to the database.", ex);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    private void AddDomainEventsAsOutboxMessages()
    {
        var outboxMessages = ChangeTracker
            .Entries<IEntity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();
                entity.ClearDomainEvents();
                return domainEvents;
            })
            .Select(domainEvent =>
            {
                var type = domainEvent.GetType();
                return new OutboxMessage(
                    Guid.NewGuid(),
                    _dateTimeProvider.UtcNow,
                    type.Name,
                    JsonSerializer.Serialize(domainEvent, type));
            })
            .ToList();

        AddRange(outboxMessages);
    }
}