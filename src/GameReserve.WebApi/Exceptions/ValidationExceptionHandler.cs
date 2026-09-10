using FluentValidation;
using MassTransit.Internals;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GameReserve.WebApi.Exceptions;
public class ValidationExceptionHandler(
    ILogger<ValidationExceptionHandler> logger,
    IProblemDetailsService problemDetailsService
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if(exception is not ValidationException validationException)
        {
            return false;
        }

        logger.LogError(exception, "Validation exception occured");

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        var context = new ProblemDetailsContext()
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails()
            {
                Title = "One or more validation errors occurred",
                Status = StatusCodes.Status400BadRequest
            }
        };

        var errors = validationException.Errors
                                        .GroupBy(x => x.PropertyName)
                                        .ToDictionary(
                                            x => x.Key,
                                            x => x.Select(e => e.ErrorMessage).ToArray()
                                        );

        context.ProblemDetails.Extensions.Add("errors", errors);

        return await problemDetailsService.TryWriteAsync(context);
    }
}