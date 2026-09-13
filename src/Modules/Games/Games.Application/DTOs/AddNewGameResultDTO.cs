using Games.Domain.Primitives;

namespace Games.Application.DTOs;
public record AddNewGameResultDTO(Guid Id, string Title, string Description);