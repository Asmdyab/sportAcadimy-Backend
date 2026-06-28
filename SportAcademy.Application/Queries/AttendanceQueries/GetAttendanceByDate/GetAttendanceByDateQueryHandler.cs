using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.AttendanceDtos;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Queries.AttendanceQueries.GetAttendanceByDate;

public class GetAttendanceByDateQueryHandler : IRequestHandler<GetAttendanceByDateQuery, Result<List<AttendanceByDateRecordDto>>>
{
    private readonly IAttendanceRepository _repository;
    private readonly string _operationType = OperationType.GetAll.ToString();

    public GetAttendanceByDateQueryHandler(IAttendanceRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<AttendanceByDateRecordDto>>> Handle(GetAttendanceByDateQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByDateAsync(request.Date, cancellationToken);
        return Result<List<AttendanceByDateRecordDto>>.Success(result, _operationType);
    }
}
