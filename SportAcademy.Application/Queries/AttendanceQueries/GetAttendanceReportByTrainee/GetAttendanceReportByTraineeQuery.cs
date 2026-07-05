using MediatR;
using SportAcademy.Application.Common.Pagination;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.AttendanceDtos;

namespace SportAcademy.Application.Queries.AttendanceQueries.GetAttendanceReportByTrainee
{
    public record GetAttendanceReportByTraineeQuery(int TraineeId, PageRequest Page)
        : IRequest<Result<PagedData<TraineeAttendanceReportDto>>>, IPaginatedRequest
    {
        public PageRequest Page { get; set; } = Page;
    }
}
