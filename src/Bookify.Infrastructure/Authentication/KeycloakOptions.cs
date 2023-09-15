namespace Bookify.Infrastructure.Authentication;

public sealed class KeycloakOptions
{
    public const string Section = "Keycloak";

    public required Uri AdminUrl { get; init; }
    public required Uri TokenUrl { get; init; }
    public required string AdminClientId { get; init; }
    public required string AdminClientSecret { get; init; }
    public required string AuthClientId { get; init; }
    public required string AuthClientSecret { get; init; }
}