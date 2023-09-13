namespace Bookify.Application.Apartments.SearchApartments;

public sealed record ApartmentResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Currency)
{
    public required AddressResponse Address { get; init; }
}