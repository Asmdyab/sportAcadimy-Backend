using FluentValidation;
using SportAcademy.Application.Commands.PaymentCommands.CreatePayment;

namespace SportAcademy.Application.Validators.PaymentValidators
{
    public class CreatePaymentValidator : AbstractValidator<CreatePaymentCommand>
    {
        public CreatePaymentValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Method)
                .IsInEnum().WithMessage("Please select a valid payment method.");

            RuleFor(x => x.BranchId)
                .ApplyIdRuleFor("Branch");
        }
    }
}
