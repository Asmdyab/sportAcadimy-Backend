using SportAcademy.Domain.Entities;

namespace SportAcademy.Application.Interfaces;

public interface IOpenRouterClient
{
    Task<string> SendAsync(string systemPrompt, string userMessage, CancellationToken cancellationToken);

    Task<string> SendMessagesAsync(IReadOnlyList<OpenAiMessage> messages, CancellationToken cancellationToken);
}
