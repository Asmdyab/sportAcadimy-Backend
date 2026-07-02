using FluentValidation;
using SportAcademy.Application.Commands.CoachCommands.CreateCoachWithEmployee;
using SportAcademy.Application.DTOs.EmployeeDtos;
using SportAcademy.Application.Validators.EmployeeValidators;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Validators.CoachValidators
{
    public class CreateCoachWithEmployeeValidator : AbstractValidator<CreateCoachWithEmployeeCommand>
    {
        private static readonly string[] ValidSkillLevels =
            Enum.GetNames<SkillLevel>()
                .Select(n => n.ToLowerInvariant())
                .Concat(["professional"])
                .ToArray();

        public CreateCoachWithEmployeeValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(c => c.SkillLevel)
                .NotEmpty().WithMessage("Skill level is required.")
                .Must(v => ValidSkillLevels.Contains(v.ToLowerInvariant()))
                .WithMessage("Invalid skill level.");

            RuleFor(c => c.SportId)
                .ApplyIdRuleFor("Sport");

            RuleFor(c => c.Employee)
                .SetValidator(new CreateEmployeeDtoValidator());
        }
    }
}
