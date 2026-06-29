using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.ChatDtos;

namespace SportAcademy.Application.Queries.ChatQueries.GetAllConversations
{
    public record GetAllConversationsQuery() : IRequest<Result<List<ChatConversationDto>>>;
}
