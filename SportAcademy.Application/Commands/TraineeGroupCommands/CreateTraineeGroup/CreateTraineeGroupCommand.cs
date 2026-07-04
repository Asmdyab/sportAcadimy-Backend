using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Commands.TraineeGroupCommands.CreateTraineeGroup
{
    public record ScheduleEntry(DayOfWeek Day, TimeOnly StartTime);

    public record CreateTraineeGroupCommand(
        string? Name,
        SkillLevel SkillLevel,
        int? MaximumCapacity,
        int? DurationInMinutes,
        Gender Gender,
        int BranchId,
        int CoachId,
        List<ScheduleEntry>? Schedules
    ) : IRequest<Result<int>>;
}
