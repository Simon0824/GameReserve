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

        return resultDTO.Match<Results<Ok<GetGamesQueryDTO>, ProblemHttpResult>>(
            onSuccess: games => TypedResults.Ok(games),
            onFailure: error => error.ToProblem());
    }

    [HttpGet("get-game-by-id")]
    public async Task<IResult> GetGameById(Guid id)
    {
        var resultDTO = await sender.Send(new GetGameByIdQuery(id));

        return resultDTO.Match<Results<Ok<GetGamesCatalogResultDTO>, ProblemHttpResult>>(
            onSuccess: game => TypedResults.Ok(game),
            onFailure: error => error.ToProblem());
    }

    [HttpDelete("delete-game")]
    public async Task<IResult> DeleteGame(Guid GameId)
    {
        var resultDTO = await sender.Send(new DeleteGameCommand(GameId));

        return resultDTO.Match<Results<Ok, ProblemHttpResult>>(
            onSuccess: () => TypedResults.Ok(),
            onFailure: error => error.ToProblem()
        );
    }
}