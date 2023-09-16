using System.Data;
using System.Text.Json;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Domain.Abstractions;
using Dapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure.Outbox;

public sealed class ProcessOutboxMessagesBackgroundService : BackgroundService
{
    private readonly IDateTimeProvider _dateTimeProvider;

    private readonly IReadOnlyDictionary<string, Type> _domainEventTypes = typeof(IDomainEvent).Assembly
        .GetTypes()
        .Where(type => typeof(IDomainEvent).IsAssignableFrom(type))
        .Where(type => type.IsClass)
        .ToDictionary(type => type.Name);

    private readonly ILogger<ProcessOutboxMessagesBackgroundService> _logger;
    private readonly OutboxOptions _outboxOptions;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public ProcessOutboxMessagesBackgroundService(
        IDateTimeProvider dateTimeProvider,
        ILogger<ProcessOutboxMessagesBackgroundService> logger,
        IOptions<OutboxOptions> outboxOptions,
        IServiceProvider serviceProvider,
        ISqlConnectionFactory sqlConnectionFactory)
    {
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _outboxOptions = outboxOptions.Value;
        _serviceProvider = serviceProvider;
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_outboxOptions.IntervalInSeconds));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ProcessOutboxMessages(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process outbox messages");
            }
        }
    }

    private async Task ProcessOutboxMessages(CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        var outboxMessages = await GetOutboxMessagesAsync(connection, transaction);
        if (!outboxMessages.Any())
        {
            return;
        }

        _logger.LogInformation("Starting to process outbox messages");

        await using var scope = _serviceProvider.CreateAsyncScope();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

        foreach (var outboxMessage in outboxMessages)
        {
            Exception? exception = null;

            try
            {
                var type = _domainEventTypes[outboxMessage.Type];
                var domainEvent = (IDomainEvent)JsonSerializer.Deserialize(outboxMessage.Content, type)!;
                await publisher.Publish(domainEvent, cancellationToken);
            }
            catch (Exception caughtException)
            {
                _logger.LogError(
                    caughtException,
                    "An error occurred while processing outbox message {MessageId}",
                    outboxMessage.Id);

                exception = caughtException;
            }

            await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
        }

        transaction.Commit();
        _logger.LogInformation("Completed processing outbox messages");
    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        var sql = $"""
                   SELECT id, type, content
                   FROM outbox_messages
                   WHERE processed_on_utc IS NULL
                   ORDER BY occurred_on_utc
                   LIMIT {_outboxOptions.BatchSize}
                   FOR UPDATE
                   """;

        var outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(sql, transaction: transaction);

        return outboxMessages.ToList();
    }

    private async Task UpdateOutboxMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        OutboxMessageResponse outboxMessage,
        Exception? exception)
    {
        const string sql = """
                           UPDATE outbox_messages
                           SET processed_on_utc = @ProcessedOnUtc,
                               error = @Error
                           WHERE id = @Id
                           """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                outboxMessage.Id,
                ProcessedOnUtc = _dateTimeProvider.UtcNow,
                Error = exception?.ToString()
            },
            transaction);
    }

    private sealed record OutboxMessageResponse(Guid Id, string Type, string Content);
}