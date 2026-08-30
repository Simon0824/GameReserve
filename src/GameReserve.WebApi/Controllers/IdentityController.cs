using Identity.Application.Commands;
using Identity.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameReserve.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityController(ISender sender) : ControllerBase
{
    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO dto)
    {
         var resultDTO = await sender.Send(new CreateUserCommand(dto.FullName, dto.Email, dto.Password));
         return Ok(resultDTO);
    }
}
