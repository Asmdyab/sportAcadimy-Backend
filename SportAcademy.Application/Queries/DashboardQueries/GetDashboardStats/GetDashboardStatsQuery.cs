using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.DashboardDtos;

namespace SportAcademy.Application.Queries.DashboardQueries.GetDashboardStats;

public record GetDashboardStatsQuery : IRequest<Result<DashboardStatsDto>>;
