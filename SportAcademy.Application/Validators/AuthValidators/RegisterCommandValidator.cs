using FluentValidation;
using SportAcademy.Application.Commands.AuthCommands.Register;

namespace SportAcademy.Application.Validators.AuthValidators
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(100).WithMessage("Username must not exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Length(8).WithMessage("Phone number must be 8 characters.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(x => x.SSN)
                .NotEmpty().WithMessage("SSN is required.")
                .Length(12).WithMessage("SSN length must be 12.");

            RuleFor(x => x.BirthDate)
                .LessThan(DateOnly.FromDateTime(DateTime.Now))
                .WithMessage("Birth date must be in the past.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender.");

            RuleFor(x => x.Nationality)
                .IsInEnum().WithMessage("Invalid nationality.");

            RuleFor(x => x.BranchId)
                .GreaterThan(0).WithMessage("Branch is required.");

            RuleFor(x => x.NationalityCategoryId)
                .GreaterThan(0).WithMessage("Nationality category is required.");
        }
    }
}