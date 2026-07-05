using MediatR;
using SportAcademy.Application.Common.Pagination;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.EnrollmentDtos;

namespace SportAcademy.Application.Queries.EnrollmentQueries.GetByTrainee
{
    public record GetEnrollmentsByTraineeQuery(int TraineeId, PageRequest Page)
        : IRequest<Result<PagedData<EnrollmentCardDto>>>, IPaginatedRequest
    {
        public PageRequest Page { get; set; } = Page;
    }
}
