namespace Bookify.Domain.Users;

public sealed record UserId(Guid Value)
{
    public static UserId New()
    {
        return new UserId(Guid.NewGuid());
    }
}