using FluentValidation;
using Identity.Application.DTOs;

namespace GameReserve.WebApi.Validators;
public class CreateUserValidator : AbstractValidator<CreateUserDTO>
{
    public CreateUserValidator()
    {
        RuleFor(user => user.FullName)
               .NotEmpty()
               .WithMessage("Name is empty")
               .MaximumLength(100);

        RuleFor(user => user.Email)
               .NotEmpty()
               .WithMessage("Email is empty")
               .EmailAddress()
               .WithMessage("Enter a valid email");

        RuleFor(user => user.Password)
               .NotEmpty()
               .WithMessage("Enter a password")
               .MinimumLength(6)
               .WithMessage("Password must contain at least 6 characters");
    }
}