using SportAcademy.Application.DTOs.ChatDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Entities;
using SportAcademy.Domain.Enums;
using SportAcademy.Application.Common.Result;

namespace SportAcademy.Application.Commands.ChatCommands.CreateConversation
{
    public class CreateConversationCommandHandler
    : IRequestHandler<CreateConversationCommand, Result<ChatConversationDto>>
    {
        private readonly IChatConversationRepository _conversationRepository;
        private readonly IUserContextService _userContext;
        private readonly IMapper _mapper;
        private readonly string _operation = OperationType.Add.ToString();

        public CreateConversationCommandHandler(
            IChatConversationRepository conversationRepository,
            IUserContextService userContext,
            IMapper mapper)
        {
            _conversationRepository = conversationRepository;
            _userContext = userContext;
            _mapper = mapper;
        }

        public async Task<Result<ChatConversationDto>> Handle(
            CreateConversationCommand request,
            CancellationToken cancellationToken)
        {
            var conversation = new ChatConversation
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                UserId = _userContext.UserId,
                CreatedAt = DateTime.UtcNow
            };

            await _conversationRepository.AddAsync(conversation, cancellationToken);

            var dto = _mapper.Map<ChatConversationDto>(conversation);

            return Result<ChatConversationDto>.Success(dto, _operation);
        }
    }
}
