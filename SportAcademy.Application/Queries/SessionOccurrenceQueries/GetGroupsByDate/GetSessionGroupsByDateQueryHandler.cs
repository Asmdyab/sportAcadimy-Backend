using MediatR;
using SportAcademy.Application.Common.Pagination;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.SessionOccurrenceDtos;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Queries.SessionOccurrenceQueries.GetGroupsByDate;

public class GetSessionGroupsByDateQueryHandler : IRequestHandler<GetSessionGroupsByDateQuery, Result<PagedData<SessionGroupCardDto>>>
{
    private readonly ISessionOccurrenceRepository _repository;
    private readonly string _operationType = OperationType.GetAll.ToString();

    public GetSessionGroupsByDateQueryHandler(ISessionOccurrenceRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PagedData<SessionGroupCardDto>>> Handle(GetSessionGroupsByDateQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetGroupsByDateAsync(request.Date, request.Page, request.TraineeGroupId, cancellationToken);
        return Result<PagedData<SessionGroupCardDto>>.Success(result, _operationType);
    }
}
