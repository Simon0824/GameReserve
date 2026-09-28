namespace SharedKernel.Domain.Abstractions;

public record Error(string Code, string? Description = null, string? StackTrace = null)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}