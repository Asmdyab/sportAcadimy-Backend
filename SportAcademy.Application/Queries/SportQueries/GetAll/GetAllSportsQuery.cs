using MediatR;
using SportAcademy.Application.Common.Pagination;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.SportDtos;

namespace SportAcademy.Application.Queries.SportQueries.GetAll
{
    public record GetAllSportsQuery()
        : IRequest<Result<IReadOnlyList<SportDto>>>;
}
