using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reservations.Application.Commands;
using Reservations.Application.DTOs;
using Reservations.Application.Queries;
using Reservations.Domain.Primitives;
using SharedKernel.Domain.Constants;

namespace GameReserve.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationsController(ISender sender) : ControllerBase
{
    [HttpPost("add-reservation")]
    public async Task<IActionResult> AddNewReservation([FromBody] AddNewReservationDTO dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(userId is null)
        {
            return Unauthorized();
        }
        
        var resultDTO = await sender.Send(new AddNewReservationCommand(
            new UserId(Guid.Parse(userId)), 
            dto.GameId, 
            dto.StartDate,
            dto.EndDate));
        return Ok(resultDTO);
    }

    [HttpGet("get-reservation-profile-by-{id:guid}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> GetReservationProfileById(Guid id)
    {
        var resultDTO = await sender.Send(new GetReservationProfileByIdQuery(id));
        return Ok(resultDTO);
    }
}