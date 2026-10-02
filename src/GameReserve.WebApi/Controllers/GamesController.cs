using GameReserve.WebApi.Extensions;
using Games.Application.Commands;
using Games.Application.DTOs;
using Games.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Domain.Constants;

namespace GameReserve.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GamesController(ISender sender) : ControllerBase
{
    [HttpPost("add-game")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IResult> AddNewGame([FromBody] AddNewGameDTO dto)
    {
        var resultDTO = await sender.Send(new AddNewGameCommand(dto.Title, dto.Description, dto.Category));


        return resultDTO.Match<Results<Ok<AddNewGameResultDTO>, ProblemHttpResult>>(
            onSuccess: game => TypedResults.Ok(game),
            onFailure: error => error.ToProblem());
    }

    [HttpGet("get-games")]
    public async Task<IResult> GetGames()
    {
        var resultDTO = await sender.Send(new GetGamesCatalogQuery());

        return resultDTO.IsSucceded ? TypedResults.Ok(resultDTO.Value) : resultDTO.Error.ToProblem();
    }

    [HttpGet("get-game-by-id")]
    public async Task<IResult> GetGameById(Guid id)
    {
        var resultDTO = await sender.Send(new GetGameByIdQuery(id));

        return resultDTO.IsSucceded ? TypedResults.Ok(resultDTO.Value) : resultDTO.Error.ToProblem();
    }
}