using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Entities;

namespace SportAcademy.Application.Services
{
    public class ChatBotService : IChatBotService
    {
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
            // read history
            var history = await _messageRepository
                .GetByConversationIdAsync(conversationId, cancellationToken);

            var aiMessages = history
                .Select(m => new OpenAiMessage
                {
                    Role = m.Role,
                    Content = m.Content
                })
                .ToList();

            // call AI via OpenRouter
            var response = await _openRouterClient.SendMessagesAsync(aiMessages, cancellationToken);

            return response;
        }
    }
}

