using FluentValidation;
using SportAcademy.Application.Commands.EmployeeCommands.UpdateEmployee;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Validators.EmployeeValidators
{
    public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Invalid employee ID.");

            When(x => x.PhoneNumber != null, () =>
            {
                RuleFor(x => x.PhoneNumber)
                    .NotEmpty().WithMessage("Phone number is required.")
                    .Matches(@"^(?:\+965)?[569]\d{7}$")
                    .WithMessage("Enter a valid Kuwaiti phone number (8 digits, starting with 5, 6, or 9).");
            });

            When(x => x.SecondPhoneNumber != null, () =>
            {
                RuleFor(x => x.SecondPhoneNumber)
                    .Matches(@"^(?:\+965)?[569]\d{7}$")
                    .WithMessage("Enter a valid secondary Kuwaiti phone number.");
            });

            When(x => x.Position != null, () =>
            {
                RuleFor(x => x.Position)
                    .NotEmpty()
                    .Must(v => Enum.TryParse<Position>(v, true, out _))
                    .WithMessage("Invalid position value.");
            });

            When(x => x.Salary != null, () =>
            {
                RuleFor(x => x.Salary)
                    .GreaterThan(0).WithMessage("Salary must be greater than zero.")
                    .LessThanOrEqualTo(100000).WithMessage("Salary seems unusually high, please double-check.");
            });

            When(x => x.BranchId != null, () =>
            {
                RuleFor(x => x.BranchId)
                    .GreaterThan(0)
                    .WithMessage("Please select a valid branch.");
            });

            When(x => x.Street != null, () =>
            {
                RuleFor(x => x.Street)
                    .NotEmpty().WithMessage("Street is required.")
                    .MaximumLength(100).WithMessage("Street can't exceed 100 characters.");
            });

            When(x => x.City != null, () =>
            {
                RuleFor(x => x.City)
                    .NotEmpty().WithMessage("City is required.")
                    .MaximumLength(50).WithMessage("City can't exceed 50 characters.");
            });

            When(x => x.Nationality != null, () =>
            {
                RuleFor(x => x.Nationality)
                    .NotEmpty()
                    .Must(v => Enum.TryParse<Nationality>(v, true, out _))
                    .WithMessage("Invalid nationality value.");
            });
        }
    }
}
