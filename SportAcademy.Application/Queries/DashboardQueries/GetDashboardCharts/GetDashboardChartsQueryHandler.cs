using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.DashboardDtos;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Queries.DashboardQueries.GetDashboardCharts;

public class GetDashboardChartsQueryHandler : IRequestHandler<GetDashboardChartsQuery, Result<DashboardChartsDto>>
{
    private readonly ISportRepository _sportRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IAttendanceRepository _attendanceRepository;

    public GetDashboardChartsQueryHandler(
        ISportRepository sportRepository,
        IEnrollmentRepository enrollmentRepository,
        IAttendanceRepository attendanceRepository)
    {
        _sportRepository = sportRepository;
        _enrollmentRepository = enrollmentRepository;
        _attendanceRepository = attendanceRepository;
    }

    public async Task<Result<DashboardChartsDto>> Handle(GetDashboardChartsQuery request, CancellationToken cancellationToken)
    {
        var months = request.Months > 0 ? request.Months : 5;
        var now = DateTime.UtcNow;
        var endDate = now.AddMonths(request.Offset);

        var monthlyAttendance = new List<MonthlyAttendanceDto>(months);
        for (var i = 0; i < months; i++)
        {
            var d = endDate.AddMonths(-(months - 1 - i));
            var month = (Month)d.Month;
            var rate = await _attendanceRepository.GetMonthlyAttendanceRate(month, cancellationToken);
            monthlyAttendance.Add(new MonthlyAttendanceDto(d.ToString("MMM"), rate));
        }

        var sports = await _sportRepository.GetAllAsync(cancellationToken);
        var from = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var enrollmentsBySport = new List<SportEnrollmentDto>(sports.Count);
        foreach (var s in sports)
        {
            var count = await _enrollmentRepository.GetEnrollmentsCountForSport(s.Id, from, DateTime.UtcNow, cancellationToken);
            if (count > 0)
                enrollmentsBySport.Add(new SportEnrollmentDto(s.Name, count));
        }
        enrollmentsBySport = enrollmentsBySport.OrderByDescending(e => e.Count).ToList();

        var charts = new DashboardChartsDto(monthlyAttendance, enrollmentsBySport);

        return Result<DashboardChartsDto>.Success(charts, "Get Dashboard Charts");
    }
}
