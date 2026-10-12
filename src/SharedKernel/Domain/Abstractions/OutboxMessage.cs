using System.Text.Json;

namespace SharedKernel.Domain.Abstractions;
public class OutboxMessage
{
    public Guid Id {get; init;}
    public required string Type {get; init;}
    public required JsonDocument Content {get; init;}
    public DateTime OccuredOnTime {get; init;}
    public DateTime? ProcessedOnTime {get; set;}
    public string? Error {get; set;}
}