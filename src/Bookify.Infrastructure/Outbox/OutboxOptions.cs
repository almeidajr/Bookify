namespace Bookify.Infrastructure.Outbox;

public sealed class OutboxOptions
{
    public const string Section = "Outbox";

    public required int IntervalInSeconds { get; init; }
    public required int BatchSize { get; init; }
}