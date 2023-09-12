using FluentValidation;

namespace Bookify.Application.Bookings.ReserveBooking;

public sealed class ReserveBookingCommandValidator : AbstractValidator<ReserveBookingCommand>
{
    public ReserveBookingCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.ApartmentId).NotEmpty();
        RuleFor(command => command.StartDate).LessThan(command => command.EndDate);
    }
}