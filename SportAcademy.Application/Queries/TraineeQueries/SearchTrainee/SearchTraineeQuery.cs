using MediatR;
using SportAcademy.Application.Common.Pagination;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.Common.Search;
using SportAcademy.Application.DTOs.TraineeDtos;

namespace SportAcademy.Application.Queries.TraineeQueries.SearchTrainee;

public record SearchTraineeQuery(
    string Term,
    PageRequest Page,
    bool? IsSubscribed = null,
    string? Sport = null
) : IRequest<Result<PagedData<TraineeCardDto>>>,
    IPaginatedRequest,
    ISearchRequest
{
    public PageRequest Page { get; set; } = Page;
}