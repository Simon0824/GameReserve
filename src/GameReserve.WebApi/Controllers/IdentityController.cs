using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameReserve.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityController(IUserRepository userRepository) : ControllerBase
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

         var resultDTO = new UserCreateResultDTO(user.Id, user.FullName, user.Email!);
         return Ok(resultDTO);
    }
}
