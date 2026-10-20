using System.Text.Json;

namespace SharedKernel.Domain.Abstractions;
public class InboxMessage
{
  public Guid Id {get; set;}
  public string Type {get; init;} = string.Empty;
  public JsonDocument Content {get; init;} = JsonDocument.Parse("{}");
  public DateTime OccuredOnTime {get; set;}
  public DateTime? ProcessedOnTime {get; set;}
  public string? Error {get; set;}  

  private InboxMessage() {}

  public InboxMessage(Guid messageId, string type, JsonDocument content, DateTime occuredOnTime)
  {
    Id = messageId;
    Type = type;
    Content = content;
    OccuredOnTime = occuredOnTime;
  }
}