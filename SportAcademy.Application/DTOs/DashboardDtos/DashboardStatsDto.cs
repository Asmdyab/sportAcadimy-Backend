namespace SportAcademy.Application.DTOs.DashboardDtos;

public record DashboardStatsDto(
    int TraineesCount,
    int ActiveCoaches,
    int TodaySessionsCount,
    int AttendanceRate
);
