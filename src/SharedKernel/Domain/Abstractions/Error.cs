using SharedKernel.Domain.Enums;

namespace SharedKernel.Domain.Abstractions;

public record Error(string Code, string? Description = null, ErrorTypes? ErrorType = null)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static Error Validation(string Code, string Description) => new(Code, Description, ErrorTypes.Validation);
    public static Error NotFound(string Code, string Description) => new(Code, Description, ErrorTypes.NotFound);
    public static Error Conflict(string Code, string Description) => new(Code, Description, ErrorTypes.Conflict);
    public static Error Forbidden(string Code, string Description) => new(Code, Description, ErrorTypes.Forbidden);
    public static Error Unauthorized(string Code, string Description) => new(Code, Description, ErrorTypes.Unauthorized);
}