using MediatR;
using SportAcademy.Application.Common.Pagination;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.SessionOccurrenceDtos;

namespace SportAcademy.Application.Queries.SessionOccurrenceQueries.GetGroupsByDate;

public record GetSessionGroupsByDateQuery(DateTime Date, PageRequest Page, int? TraineeGroupId = null) : IRequest<Result<PagedData<SessionGroupCardDto>>>;
