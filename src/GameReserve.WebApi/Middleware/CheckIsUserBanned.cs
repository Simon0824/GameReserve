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
        var header = context.Request.Headers.Authorization;
        if(!string.IsNullOrWhiteSpace(header))
        {
            var userId = context.User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value;

            var isBanned = await _blackListRepository.IsBannedAsync(userId);

            if(isBanned)
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }

        await _next(context);
    }
}

public static class CheckUserExtension
{
    public static IApplicationBuilder UseCheckIsUserBanned(
        this IApplicationBuilder builder) => builder.UseMiddleware<CheckIsUserBanned>();
}