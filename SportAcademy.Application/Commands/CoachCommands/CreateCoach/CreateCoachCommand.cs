using MediatR;
using SportAcademy.Application.Common.Result;

namespace SportAcademy.Application.Commands.CoachCommands.CreateCoach;

public record CreateCoachCommand(
    int EmployeeId,
    int SportId,
    string SkillLevel
) : IRequest<Result<int>>;
