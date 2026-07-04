using MediatR;
using SportAcademy.Application.Common.Result;

namespace SportAcademy.Application.Commands.ChatCommands.DeleteConversation
{
    public record DeleteConversationCommand(Guid Id) : IRequest<Result>;
}
