using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Enums;
using SportAcademy.Domain.Exceptions;

namespace SportAcademy.Application.Commands.ChatCommands.DeleteConversation
{
    public class DeleteConversationCommandHandler
        : IRequestHandler<DeleteConversationCommand, Result>
    {
        private readonly IChatConversationRepository _conversationRepository;
        private readonly string _operation = OperationType.Delete.ToString();

        public DeleteConversationCommandHandler(
            IChatConversationRepository conversationRepository)
        {
            _conversationRepository = conversationRepository;
        }

        public async Task<Result> Handle(
            DeleteConversationCommand request,
            CancellationToken cancellationToken)
        {
            var conversation = await _conversationRepository
                .GetByIdAsync(request.Id, cancellationToken)
                ?? throw new ChatConversationNotFoundException(request.Id.ToString());

            await _conversationRepository.DeleteAsync(conversation, cancellationToken);

            return Result.Success(_operation);
        }
    }
}
