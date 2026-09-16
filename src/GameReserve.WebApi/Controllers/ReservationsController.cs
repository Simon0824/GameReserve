using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reservations.Application.Queries;

namespace GameReserve.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationsController(ISender sender) : ControllerBase
{
    [HttpGet("get-reservation-profile-by-{id:guid}")]
    public async Task<IActionResult> GetReservationProfileById(Guid id)
    {
        var resultDTO = await sender.Send(new GetReservationProfileByIdQuery(id));
        return Ok(resultDTO);
    }
}