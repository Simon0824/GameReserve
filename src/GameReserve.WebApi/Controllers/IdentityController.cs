using Identity.Application.Commands;
using Identity.Application.DTOs;
using Identity.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Domain.Constants;

namespace GameReserve.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IdentityController(ISender sender) : ControllerBase
{
    [HttpPost("users")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO dto)
    {
         var resultDTO = await sender.Send(new CreateUserCommand(dto.FullName, dto.Email, dto.Password));
         return Ok(resultDTO);
    }

    [HttpPost("auth/login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserDTO dto)
    {
         var resultDTO = await sender.Send(new LoginUserCommand(dto.Email, dto.Password));
         return Ok(resultDTO);
    }

    [HttpPost("auth/login-with-refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginWithRefreshToken([FromBody] LoginWithRefreshTokenDTO dto)
    {
         var resultDTO = await sender.Send(new LoginWithRefreshTokenCommand(dto.RefreshToken));
         return Ok(resultDTO);
    }

    [HttpGet("get-users")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> GetUsers()
    {
         var resultDTO = await sender.Send(new GetUsersQuery());
         return Ok(resultDTO);
    }
}
