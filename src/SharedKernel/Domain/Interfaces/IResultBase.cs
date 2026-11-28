using SharedKernel.Domain.Abstractions;

namespace SharedKernel.Domain.Interfaces;

public interface IResultBase
{
    bool IsSucceeded { get; }
    bool IsFailed => !IsSucceeded;
    Error Error { get; }
}
