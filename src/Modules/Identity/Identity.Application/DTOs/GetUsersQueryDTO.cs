namespace Identity.Application.DTOs;
public record GetUsersQueryDTO(string Message, IEnumerable<GetUsersResultDTO> Users);