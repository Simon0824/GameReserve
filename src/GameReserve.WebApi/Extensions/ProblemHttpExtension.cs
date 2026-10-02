using Microsoft.AspNetCore.Http.HttpResults;
using SharedKernel.Domain.Abstractions;
using SharedKernel.Domain.Enums;

namespace GameReserve.WebApi.Extensions;
public static class ProblemHttpExtension
{
    public static ProblemHttpResult ToProblem(this Error error) => error.ErrorType switch
    {
      ErrorTypes.Validation => TypedResults.Problem(
        title: "Validation failed",
        detail: error.Description,
        statusCode: StatusCodes.Status400BadRequest,
        extensions: new Dictionary<string, object?> { ["errorCode"] = error.Code }),

      ErrorTypes.NotFound => TypedResults.Problem(
        title: "Resource not found",
        detail: error.Description,
        statusCode: StatusCodes.Status404NotFound,
        extensions: new Dictionary<string, object?> { ["errorCode"] = error.Code }),

      ErrorTypes.Conflict => TypedResults.Problem(
        title: "Conflict",
        detail: error.Description,
        statusCode: StatusCodes.Status409Conflict,
        extensions: new Dictionary<string, object?> { ["errorCode"] = error.Code }),

      ErrorTypes.Forbidden => TypedResults.Problem(
        title: "Forbidden",
        detail: error.Description,
        statusCode: StatusCodes.Status403Forbidden,
        extensions: new Dictionary<string, object?> { ["errorCode"] = error.Code }),

      ErrorTypes.Unauthorized => TypedResults.Problem(
        title: "Unauthorized",
        detail: error.Description,
        statusCode: StatusCodes.Status401Unauthorized,
        extensions: new Dictionary<string, object?> { ["errorCode"] = error.Code }),

      _ => TypedResults.Problem(
        title: "An error occurred",
        detail: error.Description,
        statusCode: StatusCodes.Status500InternalServerError,
        extensions: new Dictionary<string, object?> { ["errorCode"] = error.Code })
    };
}