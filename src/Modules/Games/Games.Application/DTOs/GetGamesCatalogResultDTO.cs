using Games.Domain.Enums;

namespace Games.Application.DTOs;
public record GetGamesCatalogResultDTO(Guid Id, string Title, string Description, GameCategory Category);