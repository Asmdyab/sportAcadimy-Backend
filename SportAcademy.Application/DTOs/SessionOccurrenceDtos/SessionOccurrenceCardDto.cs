namespace SportAcademy.Application.DTOs.SessionOccurrenceDtos;

public class SessionOccurrenceCardDto
{
    public int Id { get; set; }
    public string? SportName { get; set; }
    public string? CoachName { get; set; }
    public string? BranchName { get; set; }
    public DateTime StartTime { get; set; }
    public int DurationInMinutes { get; set; }
    public int TraineeGroupId { get; set; }
    public string? TraineeGroupName { get; set; }
    public int TraineesCount { get; set; }
    public int TotalEnrolled { get; set; }
    public int TotalPresent { get; set; }
    public int TotalAbsent { get; set; }
    public int TotalLate { get; set; }
    public DateTime Date { get; set; }
}
