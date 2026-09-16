using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reservations.Application.Queries;
using SharedKernel.Domain.Constants;

namespace GameReserve.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationsController(ISender sender) : ControllerBase
{
    [HttpGet("get-reservation-profile-by-{id:guid}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> GetReservationProfileById(Guid id)
    {
        var resultDTO = await sender.Send(new GetReservationProfileByIdQuery(id));
        return Ok(resultDTO);
    }
}