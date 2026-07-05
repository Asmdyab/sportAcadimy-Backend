using MediatR;
using SportAcademy.Application.Common.Pagination;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.AttendanceDtos;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Queries.AttendanceQueries.GetAttendanceReportByTrainee
{
    public class GetAttendanceReportByTraineeQueryHandler : IRequestHandler<GetAttendanceReportByTraineeQuery, Result<PagedData<TraineeAttendanceReportDto>>>
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly string _operationType = OperationType.GetAll.ToString();

        public GetAttendanceReportByTraineeQueryHandler(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public async Task<Result<PagedData<TraineeAttendanceReportDto>>> Handle(GetAttendanceReportByTraineeQuery request, CancellationToken cancellationToken)
        {
            var result = await _attendanceRepository.GetAttendanceReportByTraineeAsync(
                request.TraineeId, request.Page, cancellationToken);

            return Result<PagedData<TraineeAttendanceReportDto>>.Success(result, _operationType);
        }
    }
}
