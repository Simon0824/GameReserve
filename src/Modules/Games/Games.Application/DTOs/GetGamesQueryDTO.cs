namespace Games.Application.DTOs;
public record GetGamesQueryDTO(string Message, IEnumerable<GetGamesCatalogResultDTO> Games);