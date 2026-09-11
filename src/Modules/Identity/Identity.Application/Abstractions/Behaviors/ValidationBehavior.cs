using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using MediatR;

namespace Identity.Application.Abstractions.Behaviors;
public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators
) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        return await next();
    }
}