using MediatR;
using SportAcademy.Application.Common.Pagination;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.SessionOccurrenceDtos;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Queries.SessionOccurrenceQueries.SearchGroups;

public class SearchSessionGroupsQueryHandler : IRequestHandler<SearchSessionGroupsQuery, Result<PagedData<SessionGroupCardDto>>>
{
    private readonly ISessionOccurrenceRepository _repository;
    private readonly string _operationType = OperationType.GetAll.ToString();

    public SearchSessionGroupsQueryHandler(ISessionOccurrenceRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PagedData<SessionGroupCardDto>>> Handle(SearchSessionGroupsQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.SearchGroupsAsync(request.Term, request.Page, cancellationToken);
        return Result<PagedData<SessionGroupCardDto>>.Success(result, _operationType);
    }
}
