namespace SportAcademy.Domain.Entities;

public class ChatConversation
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string UserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    // Navigation Property
    public ICollection<OpenAiMessage> Messages { get; set; } = new List<OpenAiMessage>();
}
