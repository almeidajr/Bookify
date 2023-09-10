using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;

namespace Bookify.Domain.Bookings;

public sealed class PricingService
{
    public PricingDetails CalculatePrice(Apartment apartment, DateRange period)
    {
        var priceForPeriod = apartment.Price with { Amount = apartment.Price.Amount * period.LengthInDays };

        var percentageUpCharge = apartment.Amenities.Sum(amenity => amenity switch
        {
            Amenity.GardenView or Amenity.MountainView => 0.05m,
            Amenity.AirConditioning => 0.01m,
            Amenity.Parking => 0.01m,
            _ => 0
        });

        var amenitiesUpCharge = percentageUpCharge > 0
            ? apartment.Price with { Amount = priceForPeriod.Amount * percentageUpCharge }
            : Money.Zero(apartment.Price.Currency);

        var totalPrice = priceForPeriod + apartment.CleaningFee + amenitiesUpCharge;

        return new PricingDetails(priceForPeriod, apartment.CleaningFee, amenitiesUpCharge, totalPrice);
    }
}