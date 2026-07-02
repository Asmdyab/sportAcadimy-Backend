using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.EmployeeDtos;

namespace SportAcademy.Application.Commands.CoachCommands.CreateCoachWithEmployee
{
    public record CreateCoachWithEmployeeCommand(
        string SkillLevel,
        int SportId,
        CreateEmployeeDto Employee
    ) : IRequest<Result<int>>;
}
