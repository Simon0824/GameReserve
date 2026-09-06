using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Queries;
public record GetUsersQuery : IRequest<List<GetUsersResultDTO>>;
public class GetUsersQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUsersQuery, List<GetUsersResultDTO>>
{
    public async Task<List<GetUsersResultDTO>> Handle(GetUsersQuery request, CancellationToken token)
    {
        var users = await userRepository.GetUsers();
        var result = new List<GetUsersResultDTO>();
        if(users is null || !users.Any())
        {
            return result;
        }

        foreach(var user in users)
        {
            result.Add(new GetUsersResultDTO(
                user.Id,
                user.FullName,
                user.Email!,
                user.Status.ToString()
            ));
        }

        return result;
    }
}