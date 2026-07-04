using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.SubscriptionTypeDtos;

namespace SportAcademy.Application.Queries.SubscriptionTypeQueries.GetSubTypesDropdown;

public record GetSubTypesDropdownQuery() : IRequest<Result<List<SubTypeDropdownDto>>>;
