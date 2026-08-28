using Identity.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GameReserve.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityController : ControllerBase
{
    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDTO dto)
    {
         var user = Identity.Domain.UserAggregate.User.Create(dto.FullName, dto.Email);
         return Ok(user);
    }
}
