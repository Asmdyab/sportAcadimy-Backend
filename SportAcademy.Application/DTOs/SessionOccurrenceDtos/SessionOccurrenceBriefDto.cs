namespace SportAcademy.Application.DTOs.SessionOccurrenceDtos;

public class SessionOccurrenceBriefDto
{
    public int Id { get; set; }
    public DateTime StartDateTime { get; set; }
    public int TraineesCount { get; set; }
    public int TotalEnrolled { get; set; }
    public int TotalPresent { get; set; }
    public int TotalAbsent { get; set; }
    public int TotalLate { get; set; }
}
