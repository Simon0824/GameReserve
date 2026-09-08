using Games.Application.Commands;
using Games.Application.DTOs;
using Games.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> AddNewGame([FromBody] AddNewGameDTO dto)
    {
        var resultDTO = await sender.Send(new AddNewGameCommand(dto.Title, dto.Description));
        return Ok(resultDTO);
    }

    [HttpGet("get-games")]
    public async Task<IActionResult> GetGames()
    {
        var resultDTO = await sender.Send(new GetGamesCatalogQuery());
        return Ok(resultDTO);
    }
}