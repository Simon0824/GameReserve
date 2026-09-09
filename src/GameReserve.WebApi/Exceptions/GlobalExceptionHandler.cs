using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GameReserve.WebApi.Exceptions;
public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An unhadled error occured");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
        {
           Exception = exception,
           HttpContext = httpContext,
           ProblemDetails = new ProblemDetails()
           {
               Title = "An unexpected error occured",
               Detail = exception.Message,
               Type = exception.GetType().Name
           }
        });
    }
}