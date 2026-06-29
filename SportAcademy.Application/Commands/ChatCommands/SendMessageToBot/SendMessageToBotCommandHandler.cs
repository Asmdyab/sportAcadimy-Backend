using AutoMapper;
using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.ChatDtos;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Entities;
using SportAcademy.Domain.Enums;
using SportAcademy.Domain.Exceptions;

namespace SportAcademy.Application.Commands.ChatCommands.SendMessageToBot
{
    public class SendMessageToBotCommandHandler
        : IRequestHandler<SendMessageToBotCommand, Result<ChatMessageDto>>
    {
        private readonly IChatBotService _chatBotService;
        private readonly IChatMessageRepository _messageRepository;
        private readonly IChatConversationRepository _conversationRepository;
        private readonly IMapper _mapper;
        private readonly string _operation = OperationType.Add.ToString();

        public SendMessageToBotCommandHandler(
            IChatBotService chatBotService,
            IChatMessageRepository messageRepository,
            IChatConversationRepository conversationRepository,
            IMapper mapper)
        {
            _chatBotService = chatBotService;
            _messageRepository = messageRepository;
            _conversationRepository = conversationRepository;
            _mapper = mapper;
        }

        public async Task<Result<ChatMessageDto>> Handle(
            SendMessageToBotCommand request,
            CancellationToken cancellationToken)
        {
            var conversation = await _conversationRepository
                .GetByIdAsync(request.ConversationId, cancellationToken)
                ?? throw new ChatConversationNotFoundException(request.ConversationId.ToString());

            var content = request.Message?.Trim() ?? string.Empty;

            var userMessage = new OpenAiMessage
            {
                Id = Guid.NewGuid(),
                ChatConversationId = conversation.Id,
                Role = ChatRole.User,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            await _messageRepository.AddAsync(userMessage, cancellationToken);

            string reply;
            try
            {
                reply = await _chatBotService
                    .GenerateBotReplyAsync(conversation.Id, cancellationToken);
            }
            catch
            {
                await _messageRepository.DeleteAsync(userMessage, cancellationToken);
                throw;
            }

            var botMessage = new OpenAiMessage
            {
                Id = Guid.NewGuid(),
                ChatConversationId = conversation.Id,
                Role = ChatRole.Assistant,
                Content = reply,
                CreatedAt = DateTime.UtcNow
            };

            await _messageRepository.AddAsync(botMessage, cancellationToken);

            var dto = _mapper.Map<ChatMessageDto>(botMessage);

            return Result<ChatMessageDto>.Success(dto, _operation);
        }
    }
}
