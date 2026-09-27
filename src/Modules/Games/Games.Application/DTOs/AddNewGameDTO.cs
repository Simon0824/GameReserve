using Games.Domain.Enums;

namespace Games.Application.DTOs;
public record AddNewGameDTO
{
    public required string Title {get; init;}
    public required string Description {get; init;}
    public required GameCategory Category {get; init;}
}