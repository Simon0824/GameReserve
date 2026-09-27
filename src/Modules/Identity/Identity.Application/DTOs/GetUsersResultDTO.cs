using Identity.Domain.Enums;

namespace Identity.Application.DTOs;
public record GetUsersResultDTO(string Id, string FullName, string Email, UserStatus Status);