namespace Bookify.Infrastructure.Authentication;

public sealed record AuthenticationOptions(
    string Audience,
    string MetadataUrl,
    bool RequireHttpsMetadata,
    string Issuer)
{
    public const string Section = nameof(Authentication);
}