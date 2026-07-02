using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportAcademy.Application.Queries.NationalityQueries.GetNationalities;

namespace SportAcademy.Web.Controllers;

[Authorize(Roles = "Admin,Coach,Manager,Trainee")]
[Route("api/[controller]")]
[ApiController]
public class NationalitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NationalitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetNationalitiesQuery(), ct);
        return Ok(result);
    }
}
