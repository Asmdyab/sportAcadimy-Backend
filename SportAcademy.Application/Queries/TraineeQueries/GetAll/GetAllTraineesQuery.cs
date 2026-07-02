using MediatR;
using SportAcademy.Application.Common.Pagination;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.TraineeDtos;

namespace SportAcademy.Application.Queries.TraineeQueries.GetAll
{
    public record GetAllTraineesQuery(PageRequest Page, bool? IsSubscribed = null, string? Sport = null)
        : IRequest<Result<PagedData<TraineeCardDto>>>, IPaginatedRequest
    {
        public PageRequest Page { get; set; } = Page;
    }
}
