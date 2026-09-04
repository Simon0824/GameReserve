namespace Identity.Application.DTOs;
public record LoginWithRefreshTokenDTO
{
    public required string RefreshToken;
}