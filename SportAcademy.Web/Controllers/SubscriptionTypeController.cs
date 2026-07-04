using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportAcademy.Application.Queries.SubscriptionTypeQueries.GetSubTypesDropdown;

namespace SportAcademy.Web.Controllers
{
    [Authorize(Roles = "Admin,Coach,Manager")]
    [Route("api/subscription-type")]
    [ApiController]
    public class SubscriptionTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SubscriptionTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetSubTypesDropdownQuery(), ct);
            return Ok(result);
        }
    }
}
