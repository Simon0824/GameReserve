using System.IdentityModel.Tokens.Jwt;
using Identity.Domain.Interfaces;

namespace GameReserve.WebApi.Middleware;
public class CheckIsUserBanned
{
    private readonly RequestDelegate _next;

    private IBlackListRepository _blackListRepository;
    public CheckIsUserBanned(RequestDelegate next, IBlackListRepository blackListRepository)
    {
        _next = next;        
        _blackListRepository = blackListRepository;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if(context.User.Identity?.IsAuthenticated == true)  
        {
            var userId = context.User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value;

            var tokenId = context.User.FindFirst(JwtRegisteredClaimNames.Jti)!.Value;

            if(!string.IsNullOrEmpty(userId) && await _blackListRepository.IsBannedOrLoggedOutAsync(userId, tokenId))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }
        }

        await _next(context);
    }
}

public static class CheckUserExtension
{
    public static IApplicationBuilder UseCheckIsUserBanned(
        this IApplicationBuilder builder) => builder.UseMiddleware<CheckIsUserBanned>();
}