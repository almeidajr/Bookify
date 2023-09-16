namespace Bookify.Domain.Bookings;

public sealed record BookingId(Guid Value)
{
    public static BookingId New()
    {
        return new BookingId(Guid.NewGuid());
    }
}