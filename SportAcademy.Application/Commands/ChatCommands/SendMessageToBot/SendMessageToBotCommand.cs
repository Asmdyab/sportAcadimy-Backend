using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.ChatDtos;

namespace SportAcademy.Application.Commands.ChatCommands.SendMessageToBot
{
    public record SendMessageToBotCommand(
        Guid ConversationId,
        string Message
    ) : IRequest<Result<ChatMessageDto>>;
}
