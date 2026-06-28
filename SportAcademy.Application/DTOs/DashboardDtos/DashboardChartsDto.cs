namespace SportAcademy.Application.DTOs.DashboardDtos;

public record MonthlyAttendanceDto(
    string Month,
    int Rate
);

public record SportEnrollmentDto(
    string SportName,
    int Count
);

public record DashboardChartsDto(
    List<MonthlyAttendanceDto> MonthlyAttendance,
    List<SportEnrollmentDto> EnrollmentsBySport
);
