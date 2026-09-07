using Games.Domain.Primitives;

namespace Games.Application.DTOs;
public record AddNewGameResultDTO(GameId Id, string Title, string Description);