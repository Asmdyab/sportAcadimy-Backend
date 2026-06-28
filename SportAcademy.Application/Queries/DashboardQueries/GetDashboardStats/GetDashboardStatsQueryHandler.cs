using MediatR;
using SportAcademy.Application.Common.Pagination;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.DashboardDtos;
using SportAcademy.Application.Interfaces;

namespace SportAcademy.Application.Queries.DashboardQueries.GetDashboardStats;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
{
    private readonly ITraineeRepository _traineeRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAttendanceRepository _attendanceRepository;

    public GetDashboardStatsQueryHandler(
        ITraineeRepository traineeRepository,
        IEmployeeRepository employeeRepository,
        IAttendanceRepository attendanceRepository)
    {
        _traineeRepository = traineeRepository;
        _employeeRepository = employeeRepository;
        _attendanceRepository = attendanceRepository;
    }

    public async Task<Result<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var traineesCount = await _traineeRepository.CountAsync(cancellationToken);
        var activeCoaches = await _employeeRepository.GetActiveCoachesCountAsync(cancellationToken);
        var attendanceRate = await _attendanceRepository.GetGlobalAttendanceRate(cancellationToken);

        var today = DateTime.Today;
        var todayAttendance = await _attendanceRepository.GetByDateAsync(today, cancellationToken);
        var todaySessionsCount = todayAttendance.Select(a => a.SessionOccurrenceId).Distinct().Count();

        var stats = new DashboardStatsDto(
            TraineesCount: traineesCount,
            ActiveCoaches: activeCoaches,
            TodaySessionsCount: todaySessionsCount,
            AttendanceRate: attendanceRate
        );

        return Result<DashboardStatsDto>.Success(stats, "Get Dashboard Stats");
    }
}
