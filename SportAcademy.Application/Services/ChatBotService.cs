using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Entities;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Services
{
    public class ChatBotService : IChatBotService
    {
        private const string SystemPrompt =
            "You are a professional sports training assistant for SportAcademy. " +
            "Help users with sports training advice, exercise techniques, and fitness guidance. " +
            "Keep responses concise, accurate, and supportive.";

        private readonly IChatMessageRepository _messageRepository;
        private readonly IOpenRouterClient _openRouterClient;

        public ChatBotService(
            IChatMessageRepository messageRepository,
            IOpenRouterClient openRouterClient)
        {
            _messageRepository = messageRepository;
            _openRouterClient = openRouterClient;
        }

        public async Task<string> GenerateBotReplyAsync(
            Guid conversationId,
            CancellationToken cancellationToken)
        {
            var history = await _messageRepository
                .GetByConversationIdAsync(conversationId, cancellationToken);

            var aiMessages = new List<OpenAiMessage>
            {
                new() { Role = ChatRole.System, Content = SystemPrompt }
            };

            aiMessages.AddRange(history
                .Where(m => !string.IsNullOrWhiteSpace(m.Content))
                .Select(m => new OpenAiMessage
                {
                    Role = m.Role,
                    Content = m.Content
                }));

            if (aiMessages.Count <= 1)
                throw new InvalidOperationException("Cannot send an empty message to the AI provider.");

            var response = await _openRouterClient.SendMessagesAsync(aiMessages, cancellationToken);

            return response;
        }
    }
}

