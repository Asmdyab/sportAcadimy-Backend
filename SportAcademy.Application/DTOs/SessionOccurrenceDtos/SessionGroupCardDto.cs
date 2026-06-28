namespace SportAcademy.Application.DTOs.SessionOccurrenceDtos;

public class SessionGroupCardDto
{
    public int TraineeGroupId { get; set; }
    public string? TraineeGroupName { get; set; }
    public string? SportName { get; set; }
    public string? CoachName { get; set; }
    public string? BranchName { get; set; }
    public int DurationInMinutes { get; set; }
    public List<SessionOccurrenceBriefDto> Occurrences { get; set; } = new();
}
