namespace Bookify.Infrastructure.Authentication;

public sealed class AuthenticationOptions
{
    public const string Section = "Authentication";

    public required string Audience { get; init; }
    public required string MetadataUrl { get; init; }
    public required string ValidIssuer { get; init; }
    public required bool RequireHttpsMetadata { get; init; }
}