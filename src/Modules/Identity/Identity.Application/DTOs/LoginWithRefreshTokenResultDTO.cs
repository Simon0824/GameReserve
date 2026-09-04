namespace Identity.Application.DTOs;
public record LoginWithRefreshTokenResultDTO(string AccessToken, string RefreshToken);