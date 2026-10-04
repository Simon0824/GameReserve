namespace Identity.Application.DTOs;
public record ChangePasswordDTO
{
    public required string CurrentPassword {get; init;}
    public required string NewPassword {get; init;}
}