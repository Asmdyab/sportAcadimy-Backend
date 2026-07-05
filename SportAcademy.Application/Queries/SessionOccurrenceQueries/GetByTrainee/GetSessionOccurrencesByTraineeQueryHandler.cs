using MediatR;
using SportAcademy.Application.Common.Pagination;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.SessionOccurrenceDtos;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Queries.SessionOccurrenceQueries.GetByTrainee
{
    public class GetSessionOccurrencesByTraineeQueryHandler : IRequestHandler<GetSessionOccurrencesByTraineeQuery, Result<PagedData<SessionOccurrenceCardDto>>>
    {
        private readonly ISessionOccurrenceRepository _repository;
        private readonly string _operationType = OperationType.GetAll.ToString();

        public GetSessionOccurrencesByTraineeQueryHandler(ISessionOccurrenceRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<PagedData<SessionOccurrenceCardDto>>> Handle(GetSessionOccurrencesByTraineeQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetByTraineeIdAsync(request.TraineeId, request.Page, cancellationToken);
            return Result<PagedData<SessionOccurrenceCardDto>>.Success(result, _operationType);
        }
    }
}
