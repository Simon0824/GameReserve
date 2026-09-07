using Games.Application.Commands;
using Games.Application.DTOs;
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
}