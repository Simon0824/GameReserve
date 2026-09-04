namespace Identity.Application.DTOs;
public record CreateUserDTO
{
    public required string FullName {get; init;}
    public required string Email {get; init;}
    public required string Password {get; init;}
}