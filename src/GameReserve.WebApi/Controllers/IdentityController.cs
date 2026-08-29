using Identity.Application.DTOs;
using Identity.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GameReserve.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityController(UserRepository userRepository) : ControllerBase
{
    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDTO dto)
    {
         var user = Identity.Domain.UserAggregate.User.Create(dto.FullName, dto.Email);
         var result = await userRepository.CreateUser(user, dto.Password);
         if(!result.Succeeded)
         {
            throw new Exception("Cannot create a user");
         }

         return Ok(user);
    }
}
