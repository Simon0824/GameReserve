using FluentValidation;
using Games.Application.DTOs;

namespace GameReserve.WebApi.Validators.Games;
public class AddNewGameValidator : AbstractValidator<AddNewGameDTO>
{
    public AddNewGameValidator()
    {
        RuleFor(game => game.Title)
               .NotEmpty()
               .WithMessage("Title is empty")
               .MaximumLength(100)
               .WithMessage("Title can not be longer than 100 characters");

        RuleFor(game => game.Description)
               .NotEmpty()
               .WithMessage("Description is empty")
               .MaximumLength(1000)
               .WithMessage("Description can not be longer than 1000 characters");
    }
}