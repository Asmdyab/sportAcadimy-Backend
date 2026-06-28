using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.DashboardDtos;

namespace SportAcademy.Application.Queries.DashboardQueries.GetDashboardCharts;

public record GetDashboardChartsQuery(int Months, int Offset = 0) : IRequest<Result<DashboardChartsDto>>;
