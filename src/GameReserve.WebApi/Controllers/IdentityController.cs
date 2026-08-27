using Identity.Application.DTOs;
using Identity.Domain;
using Microsoft.AspNetCore.Mvc;

namespace GameReserve.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityController : ControllerBase
{
    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDTO dto)
    {
         var user = Identity.Domain.User.Create(Guid.NewGuid(), dto.FullName, dto.Email, dto.Password);
         return Ok(user);
    }
}
