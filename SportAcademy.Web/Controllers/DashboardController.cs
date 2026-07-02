using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportAcademy.Application.Queries.DashboardQueries.GetDashboardCharts;
using SportAcademy.Application.Queries.DashboardQueries.GetDashboardStats;

namespace SportAcademy.Web.Controllers
{
    [Authorize(Roles = "Admin,Coach,Manager,Trainee")]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetDashboardStatsQuery(), ct);
            return Ok(result);
        }

        [HttpGet("charts")]
        public async Task<IActionResult> GetCharts(
            [FromQuery] int months = 5,
            [FromQuery] int offset = 0,
            CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetDashboardChartsQuery(months, offset), ct);
            return Ok(result);
        }
    }
}
