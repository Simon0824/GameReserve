using System.Security.Claims;
using GameReserve.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public async Task<IResult> AddNewReservation([FromBody] AddNewReservationDTO dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(userId is null)
        {
            return TypedResults.Unauthorized();
        }
        
        var resultDTO = await sender.Send(new AddNewReservationCommand(
            new UserId(Guid.Parse(userId)), 
            dto.GameId, 
            dto.StartDate,
            dto.EndDate));

        return resultDTO.Match<Results<Ok<AddNewReservationResultDTO>, ProblemHttpResult>>(
            onSuccess: reservation => TypedResults.Ok(reservation),
            onFailure: error => error.ToProblem()
        );
    }

    [HttpGet("get-reservation-profile-by-{id:guid}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IResult> GetReservationProfileById(Guid id)
    {
        var resultDTO = await sender.Send(new GetReservationProfileByIdQuery(id));

        return resultDTO.IsSucceded ? TypedResults.Ok(resultDTO.Value) : resultDTO.Error.ToProblem();
    }

    [HttpGet("get-reservation-by-{id:guid}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IResult> GetReservationById(Guid id)
    {
        var resultDTO = await sender.Send(new GetReservationByIdQuery(id));

        return resultDTO.IsSucceded ? TypedResults.Ok(resultDTO.Value) : resultDTO.Error.ToProblem();
    }

    [HttpGet("get-reservation-profiles")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IResult> GetReservationProfiles()
    {
        var resultDTO = await sender.Send(new GetReservationProfilesQuery());

        return resultDTO.IsSucceded ? TypedResults.Ok(resultDTO.Value) : resultDTO.Error.ToProblem();
    }
}