using MediatR;
using SportAcademy.Application.Common.Pagination;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.SessionOccurrenceDtos;

namespace SportAcademy.Application.Queries.SessionOccurrenceQueries.SearchGroups;

public record SearchSessionGroupsQuery(string Term, PageRequest Page) : IRequest<Result<PagedData<SessionGroupCardDto>>>;
