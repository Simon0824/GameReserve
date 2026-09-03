using Identity.Domain.Entites;

namespace Identity.Application.DTOs;
public record LoginUserResultDTO(string FullName, string Email, string Token, string RefreshToken);
