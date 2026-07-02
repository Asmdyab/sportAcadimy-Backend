using FluentValidation;
using SportAcademy.Application.Commands.CoachCommands.CreateCoach;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Validators.CoachValidators
{
    public class CreateCoachValidator : AbstractValidator<CreateCoachCommand>
    {
        private static readonly string[] ValidSkillLevels =
            Enum.GetNames<SkillLevel>()
                .Select(n => n.ToLowerInvariant())
                .Concat(["professional"])
                .ToArray();

        public CreateCoachValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(c => c.SkillLevel)
                .NotEmpty().WithMessage("Skill level is required.")
                .Must(v => ValidSkillLevels.Contains(v.ToLowerInvariant()))
                .WithMessage("Invalid skill level.");

            RuleFor(c => c.SportId)
                .ApplyIdRuleFor("Sport");

            RuleFor(c => c.EmployeeId)
                .ApplyIdRuleFor("Employee");
        }
    }
}
