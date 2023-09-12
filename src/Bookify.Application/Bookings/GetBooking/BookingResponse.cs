using Bookify.Domain.Bookings;

namespace Bookify.Application.Bookings.GetBooking;

public sealed record BookingResponse(
    Guid Id,
    Guid UserId,
    Guid ApartmentId,
    BookingStatus Status,
    decimal PriceAmount,
    string PriceCurrency,
    decimal CleaningFeeAmount,
    string CleaningFeeCurrency,
    decimal AmenitiesUpChargeAmount,
    string AmenitiesUpChargeCurrency,
    decimal TotalPriceAmount,
    string TotalPriceCurrency,
    DateOnly DurationStart,
    DateOnly DurationEnd,
    DateTime CreatedOnUtc);