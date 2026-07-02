using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.EmployeeDtos;

namespace SportAcademy.Application.Commands.EmployeeCommands.UpdateEmployee
{
    public record UpdateEmployeeCommand(
        int Id,
        string? PhoneNumber = null,
        string? SecondPhoneNumber = null,
        string? Position = null,
        decimal? Salary = null,
        int? BranchId = null,
        string? Street = null,
        string? City = null,
        string? Nationality = null
    ) : IRequest<Result<EmployeeDto>>;
}
