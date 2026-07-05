using MediatR;
using SportAcademy.Application.Common.Pagination;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.SessionOccurrenceDtos;

namespace SportAcademy.Application.Queries.SessionOccurrenceQueries.GetByTrainee
{
    public record GetSessionOccurrencesByTraineeQuery(int TraineeId, PageRequest Page)
        : IRequest<Result<PagedData<SessionOccurrenceCardDto>>>, IPaginatedRequest
    {
        public PageRequest Page { get; set; } = Page;
    }
}
