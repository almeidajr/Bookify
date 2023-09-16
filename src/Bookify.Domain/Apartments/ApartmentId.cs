namespace Bookify.Domain.Apartments;

public record ApartmentId(Guid Value)
{
    public static ApartmentId New()
    {
        return new ApartmentId(Guid.NewGuid());
    }
}