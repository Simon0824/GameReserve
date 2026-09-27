using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using MediatR;

namespace Identity.Application.Queries;
public record GetUsersQuery : IRequest<IEnumerable<GetUsersResultDTO>>;
public class GetUsersQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUsersQuery, IEnumerable<GetUsersResultDTO>>
{
    public async Task<IEnumerable<GetUsersResultDTO>> Handle(GetUsersQuery request, CancellationToken token)
    {
        var users = await userRepository.GetUsers();
        var result = new List<GetUsersResultDTO>();
        if(users is null || !users.Any())
        {
            throw new Exception("Theres no users");
        }

        foreach(var user in users)
        {
            result.Add(new GetUsersResultDTO(
                user.Id,
                user.FullName,
                user.Email!,
                user.Status
            ));
        }

        return result;
    }
}