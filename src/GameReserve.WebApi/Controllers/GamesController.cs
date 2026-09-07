using Games.Application.Commands;
using Games.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameReserve.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class GamesController(ISender sender) : ControllerBase
{
    [HttpPost("add-game")]
    public async Task<IActionResult> AddNewGame([FromBody] AddNewGameDTO dto)
    {
        var resultDTO = await sender.Send(new AddNewGameCommand(dto.Title, dto.Description));
        return Ok(resultDTO);
    }
}