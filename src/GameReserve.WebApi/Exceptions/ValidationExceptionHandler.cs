using Microsoft.AspNetCore.Diagnostics;

namespace GameReserve.WebApi.Exceptions;
public class ValidationExceptionHandler() : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        return true;
    }
}