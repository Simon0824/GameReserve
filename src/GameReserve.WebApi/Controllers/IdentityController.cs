using GameReserve.WebApi.Extensions;
using Identity.Application.Commands;
using Identity.Application.DTOs;
using Identity.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public async Task<IResult> CreateUser([FromBody] CreateUserDTO dto)
    {
         var resultDTO = await sender.Send(new CreateUserCommand(dto.FullName, dto.Email, dto.Password));

         return resultDTO.Match<Results<Ok<CreateUserResultDTO>, ProblemHttpResult>>(
          onSuccess: user => TypedResults.Ok(user),
          onFailure: error => error.ToProblem()
         );
    }

    [HttpPost("auth/login")]
    [AllowAnonymous]
    public async Task<IResult> LoginUser([FromBody] LoginUserDTO dto)
    {
         var resultDTO = await sender.Send(new LoginUserCommand(dto.Email, dto.Password));

         return resultDTO.Match<Results<Ok<LoginUserResultDTO>, ProblemHttpResult>>(
          onSuccess: logged => TypedResults.Ok(logged),
          onFailure: error => error.ToProblem()
         );
    }

    [HttpPost("auth/login-with-refresh-token")]
    [AllowAnonymous]
    public async Task<IResult> LoginWithRefreshToken([FromBody] LoginWithRefreshTokenDTO dto)
    {
         var resultDTO = await sender.Send(new LoginWithRefreshTokenCommand(dto.RefreshToken));

         return resultDTO.Match<Results<Ok<LoginWithRefreshTokenResultDTO>, ProblemHttpResult>>(
          onSuccess: logged => TypedResults.Ok(logged),
          onFailure: error => error.ToProblem()
         );
    }

    [HttpPost("ban-user")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IResult> BanUser(string Email)
    {
        var resultDTO = await sender.Send(new BanUserCommand(Email));

        return resultDTO.Match<Results<Ok, ProblemHttpResult>>(
          onSuccess: () => TypedResults.Ok(),
          onFailure: error => error.ToProblem()
        );
    }

    [HttpGet("get-users")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IResult> GetUsers()
    {
         var resultDTO = await sender.Send(new GetUsersQuery());

         return resultDTO.IsSucceeded ? TypedResults.Ok(resultDTO.Value) : resultDTO.Error.ToProblem();
    }
}
