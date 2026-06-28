namespace SportAcademy.Application.DTOs.AttendanceDtos;

public class AttendanceByDateRecordDto
{
    public int Id { get; set; }
    public int SessionOccurrenceId { get; set; }
    public string? SportName { get; set; }
    public string? CoachName { get; set; }
    public string? BranchName { get; set; }
    public DateTime StartTime { get; set; }
    public int DurationInMinutes { get; set; }
    public int TraineeId { get; set; }
    public string? TraineeName { get; set; }
    public string? CheckInTime { get; set; }
    public string Status { get; set; } = "Absent";
    public DateTime AttendanceDate { get; set; }
}
