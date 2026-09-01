namespace Identity.Application.DTOs;
public record CreateUserResultDTO(
    string Id, 
    string FullName, 
    string Email
    );