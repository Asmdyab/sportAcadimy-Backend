using SportAcademy.Application.DTOs.ChatDtos;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Entities;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Services
{
    public class ChatBotService : IChatBotService
    {
        private const string SystemPrompt =
            "You are an AI assistant for SportAcademy, a sports training academy. " +
            "Your role is to help users with academy-related questions only.\n\n" +
            "ABOUT SPORTACADEMY:\n" +
            "- A sports training academy offering various sports programs\n" +
            "- Has multiple branches with different sports available at each\n" +
            "- Offers different subscription types with pricing per sport and branch\n" +
            "- Trainees can enroll in training groups led by coaches\n" +
            "- Tracks attendance, payments, and training progress\n\n" +
            "WHAT YOU CAN HELP WITH:\n" +
            "- Sports offered, their categories, descriptions, and prices\n" +
            "- Branch locations and what sports are available at each\n" +
            "- Trainee information, enrollment status, and subscriptions\n" +
            "- Training group schedules and session timings\n" +
            "- Coach details and which sports they teach\n" +
            "- Training tips, exercise techniques, and fitness guidance\n" +
            "- General questions about academy services and programs\n\n" +
            "RULES:\n" +
            "1. ONLY answer questions related to SportAcademy and sports training.\n" +
            "2. For any question outside this scope, respond with:\n" +
            "   \"I'm a SportAcademy assistant and can only help with questions related to our academy and sports training. Please ask me about sports, training programs, schedules, or other academy services.\"\n" +
            "3. Use the available tools/functions to look up real data from the academy database when answering data-related questions.\n" +
            "4. Keep responses concise, accurate, and supportive.\n" +
            "5. When you need information, call the appropriate function. The results will be provided to you automatically.\n" +
            "6. Do not make up data — only use information returned from function calls.";

        private const int MaxToolCallIterations = 5;

        private readonly IChatMessageRepository _messageRepository;
        private readonly IOpenRouterClient _openRouterClient;
        private readonly IToolRegistry _toolRegistry;

        public ChatBotService(
            IChatMessageRepository messageRepository,
            IOpenRouterClient openRouterClient,
            IToolRegistry toolRegistry)
        {
            _messageRepository = messageRepository;
            _openRouterClient = openRouterClient;
            _toolRegistry = toolRegistry;
        }

        public async Task<string> GenerateBotReplyAsync(
            Guid conversationId,
            CancellationToken cancellationToken)
        {
            var history = await _messageRepository
                .GetByConversationIdAsync(conversationId, cancellationToken);

            var messages = BuildMessageList(history);
            var tools = _toolRegistry.GetAllTools();

            var response = await _openRouterClient.SendWithToolsAsync(messages, tools, cancellationToken);

            for (int iteration = 0; iteration < MaxToolCallIterations; iteration++)
            {
                if (!HasValidChoice(response))
                    break;

                var choice = response.Choices[0];

                if (choice.Message?.ToolCalls is not { Count: > 0 })
                    return choice.Message?.Content ?? string.Empty;

                var assistantMsg = new ChatApiMessage
                {
                    Role = "assistant",
                    Content = choice.Message.Content,
                    ToolCalls = choice.Message.ToolCalls
                };
                messages.Add(assistantMsg);

                foreach (var toolCall in choice.Message.ToolCalls)
                {
                    var resultJson = await _toolRegistry.ExecuteToolAsync(
                        toolCall.Function.Name,
                        toolCall.Function.Arguments,
                        cancellationToken);

                    messages.Add(new ChatApiMessage
                    {
                        Role = "tool",
                        ToolCallId = toolCall.Id,
                        Content = resultJson
                    });
                }

                response = await _openRouterClient.SendWithToolsAsync(messages, tools, cancellationToken);
            }

            return HasValidChoice(response)
                ? response.Choices[0].Message?.Content ?? string.Empty
                : string.Empty;
        }

        private static bool HasValidChoice(OpenRouterResponseDto response)
        {
            return response.Choices is { Count: > 0 } && response.Choices[0].Message != null;
        }

        private static List<ChatApiMessage> BuildMessageList(IReadOnlyList<OpenAiMessage> history)
        {
            var messages = new List<ChatApiMessage>
            {
                new()
                {
                    Role = "system",
                    Content = SystemPrompt
                }
            };

            messages.AddRange(history
                .Where(m => !string.IsNullOrWhiteSpace(m.Content))
                .Select(m => new ChatApiMessage
                {
                    Role = m.Role.ToString().ToLowerInvariant(),
                    Content = m.Content
                }));

            if (messages.Count <= 1)
                throw new InvalidOperationException("Cannot send an empty message to the AI provider.");

            return messages;
        }
    }
}
