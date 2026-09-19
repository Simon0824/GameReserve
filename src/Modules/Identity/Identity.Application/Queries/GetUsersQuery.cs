using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Queries;
public record GetUsersQuery : IRequest<IEnumerable<GetUsersResultDTO>>;
public class GetUsersQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUsersQuery, IEnumerable<GetUsersResultDTO>>
{
    public async Task<IEnumerable<GetUsersResultDTO>> Handle(GetUsersQuery request, CancellationToken token)
    {
        var users = await userRepository.GetUsers();
        var result = Enumerable.Empty<GetUsersResultDTO>();
        if(users is null || !users.Any())
        {
            return result;
        }

        foreach(var user in users)
        {
            result.Append(new GetUsersResultDTO(
                user.Id,
                user.FullName,
                user.Email!,
                user.Status.ToString()
            ));
        }

        return result;
    }
}