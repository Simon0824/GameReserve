using FluentValidation;
using Reservations.Application.DTOs;

namespace GameReserve.WebApi.Validators.Reservations;
public class AddNewReservationValidator : AbstractValidator<AddNewReservationDTO>
{
    public AddNewReservationValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required.")
            .LessThan(x => x.EndDate)
            .WithMessage("Start date must be before end date.");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required.");

        RuleFor(x => x.GameId)
            .NotEmpty()
            .WithMessage("Game id is required.");

    }
}