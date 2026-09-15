using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reservations.Application.Queries;

namespace GameReserve.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ReservationsController(ISender sender) : ControllerBase
{
    [HttpGet("get-reservation-profile-by-{id:guid}")]
    public async Task<IActionResult> GetReservationProfileById(Guid id)
    {
        var resultDTO = await sender.Send(new GetReservationProfileByIdQuery(id));
        return Ok(resultDTO);
    }
}