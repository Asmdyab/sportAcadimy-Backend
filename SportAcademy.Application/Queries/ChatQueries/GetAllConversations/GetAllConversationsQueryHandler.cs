using AutoMapper;
using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.ChatDtos;
using SportAcademy.Application.Interfaces;

namespace SportAcademy.Application.Queries.ChatQueries.GetAllConversations
{
    public class GetAllConversationsQueryHandler
        : IRequestHandler<GetAllConversationsQuery, Result<List<ChatConversationDto>>>
    {
        private readonly IChatConversationRepository _repository;
        private readonly IUserContextService _userContext;
        private readonly IMapper _mapper;
        private readonly string _operation = "Get";

        public GetAllConversationsQueryHandler(
            IChatConversationRepository repository,
            IUserContextService userContext,
            IMapper mapper)
        {
            _repository = repository;
            _userContext = userContext;
            _mapper = mapper;
        }

        public async Task<Result<List<ChatConversationDto>>> Handle(
            GetAllConversationsQuery request,
            CancellationToken cancellationToken)
        {
            var conversations = await _repository
                .GetByUserIdAsync(_userContext.UserId, cancellationToken);

            var dtos = _mapper.Map<List<ChatConversationDto>>(conversations);

            return Result<List<ChatConversationDto>>.Success(dtos, _operation);
        }
    }
}
