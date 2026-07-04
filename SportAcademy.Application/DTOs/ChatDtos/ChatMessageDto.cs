using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.DTOs.ChatDtos;

public class ChatMessageDto
{
    public Guid Id { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
