using System.Text.Json.Serialization;

namespace Bookify.Infrastructure.Authentication.Models;

public sealed record AuthorizationToken(
    [property: JsonPropertyName("access_token")]
    string AccessToken);