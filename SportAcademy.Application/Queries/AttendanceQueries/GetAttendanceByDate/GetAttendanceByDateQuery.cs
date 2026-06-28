using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.AttendanceDtos;

namespace SportAcademy.Application.Queries.AttendanceQueries.GetAttendanceByDate;

public record GetAttendanceByDateQuery(DateTime Date) : IRequest<Result<List<AttendanceByDateRecordDto>>>;
