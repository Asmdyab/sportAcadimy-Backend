using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Entities;
using SportAcademy.Domain.Enums;
using SportAcademy.Domain.Exceptions.EmployeeExceptions;

namespace SportAcademy.Application.Commands.CoachCommands.CreateCoach
{
    public class CreateCoachCommandHandler : IRequestHandler<CreateCoachCommand, Result<int>>
    {
        private readonly string _operationType = OperationType.Add.ToString();
        private readonly ICoachRepository _coachRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public CreateCoachCommandHandler(
            IEmployeeRepository employeeRepository,
            ICoachRepository coachRepository)
        {
            _employeeRepository = employeeRepository;
            _coachRepository = coachRepository;
        }

        public async Task<Result<int>> Handle(CreateCoachCommand request, CancellationToken ct)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, ct)
                ?? throw new EmployeeNotFoundException(request.EmployeeId.ToString());

            ct.ThrowIfCancellationRequested();

            var existingCoach = await _coachRepository.GetByIdIncludeDeletedAsync(request.EmployeeId, ct);

            if (existingCoach != null)
            {
                existingCoach.IsDeleted = false;
                existingCoach.DeletedAt = null;
                existingCoach.DeletedBy = null;
                existingCoach.SportId = request.SportId;
                existingCoach.SkillLevel = ParseSkillLevel(request.SkillLevel);

                await _coachRepository.UpdateAsync(existingCoach, ct);

                return Result<int>.Success(employee.Id, _operationType);
            }

            var coach = new Coach
            {
                EmployeeId = request.EmployeeId,
                SportId = request.SportId,
                SkillLevel = ParseSkillLevel(request.SkillLevel)
            };

            ct.ThrowIfCancellationRequested();

            await _coachRepository.AddAsync(coach, ct);

            return Result<int>.Success(employee.Id, _operationType);
        }

        private static SkillLevel ParseSkillLevel(string value)
        {
            if (value.Equals("Professional", StringComparison.OrdinalIgnoreCase))
                return SkillLevel.Expert;

            return Enum.Parse<SkillLevel>(value, ignoreCase: true);
        }
    }
}
