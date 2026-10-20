using Games.Domain.Enums;

namespace Games.Application.DTOs;
public record AddNewGameResultDTO(Guid Id, string Title, string Description, GameCategory GameCategory);